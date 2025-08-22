using MoreInjuries.Caching;
using MoreInjuries.Defs.WellKnown;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MoreInjuries.HealthConditions.Choking;

public sealed class HediffComp_Choking : HediffComp
{
    private readonly TimedDataField<HediffComp_Choking, bool, Hediff_Injury, TimedDataEntry<bool>> _sourceIsProbablyValid;
    private Std::WeakReference<Hediff_Injury>? _source;

    public HediffCompProperties_Choking Properties => (HediffCompProperties_Choking)props;

    public HediffComp_Choking()
    {
        _sourceIsProbablyValid = new TimedDataField<HediffComp_Choking, bool, Hediff_Injury, TimedDataEntry<bool>>
        (
            owner: this,
            minRefreshIntervalTicks: GenTicks.TickRareInterval,
            dataProvider: static (self, source) =>
                // the object may not have been GC'ed yet, but that doesn't mean it's valid
                self.parent.pawn.health.hediffSet.hediffs.Contains(source)
        );
    }

    public override void CompPostMake()
    {
        if (MoreInjuriesMod.Settings.EnableChokingSounds)
        {
            KnownSoundDefOf.Choking.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
        }
    }

    public Hediff_Injury? GetSource(bool validate)
    {
        // attempt to materialize the reference to our source
        if (_source is null || !_source.TryGetTarget(out Hediff_Injury? source))
        {
            return null;
        }
        if (!_sourceIsProbablyValid.GetData(source, validate))
        {
            // got a dead reference
            Logger.LogDebug($"Invalidating dead reference to source hediff {source.def.label}");
            source = null;
            _source = null;
        }
        return source;
    }

    public void SetSource(Hediff_Injury? value)
    {
        if (value is null)
        {
            _source = null;
        }
        else
        {
            _source = new Std::WeakReference<Hediff_Injury>(value);
        }
    }

    private static bool IsCoughing(Hediff? source, Pawn pawn) =>
        source is not { Bleeding: true } && (pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness) > 0.3f
        || ModLister.BiotechInstalled && pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest));

    public override string CompLabelInBracketsExtra => IsCoughing(GetSource(validate: false), parent.pawn)
        ? "MI_Coughing".Translate()
        : string.Empty;

    public override void CompExposeData()
    {
        base.CompExposeData();
        Hediff_Injury? source = GetSource(validate: true);
        Hediff_Injury? oldSource = source;
        Scribe_References.Look(ref source, "chokingSource");
        if (!ReferenceEquals(source, oldSource))
        {
            SetSource(source);
        }
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (!parent.pawn.IsHashIntervalTick(Properties.ChokingIntervalTicks))
        {
            return;
        }
        Pawn patient = parent.pawn;
        Hediff_Injury? source = GetSource(validate: true);
        // a random walk with a bias towards increasing severity, increase depends on the bleed rate of the source injury and whether the patient is tended
        float increase = 0.1f;
        float decrease = 0f;
        if (source is { BleedRate: > 0.01f })
        {
            increase += Mathf.Clamp(source.BleedRate / 5f, 0.05f, 0.25f);
        }
        else if (source is null || source.IsTended())
        {
            decrease = 0.075f;
        }
        else if (source.BleedRate <= 0.01f)
        {
            decrease = 0.025f;
        }
        float change = Rand.Range(-decrease, increase);
        bool coughing = IsCoughing(source, patient);
        if (coughing)
        {
            // The patient is conscious and coughing, so the severity decreases faster.
            // The range (0.05f to 0.15f) was chosen to balance the impact of coughing on severity reduction.
            // If this range needs adjustment, consider its effect on game balance and frequency of coughing.
            change -= Rand.Range(0.05f, 0.15f);
        }
        float newSeverity = Mathf.Clamp01(parent.Severity + change);
        if (newSeverity > Mathf.Epsilon)
        {
            parent.Severity = newSeverity;
            if (MoreInjuriesMod.Settings.EnableChokingSounds)
            {
                SoundDef soundDef = (coughing, patient.gender) switch
                {
                    (true, Gender.Female) => KnownSoundDefOf.ChokingCoughFemale,
                    (true, _) => KnownSoundDefOf.ChokingCoughMale,
                    _ => KnownSoundDefOf.Choking,
                };
                soundDef.PlayOneShot(SoundInfo.InMap(patient, MaintenanceType.None));
            }
        }
        else
        {
            patient.health.RemoveHediff(parent);
        }
    }
}
