using MoreInjuries.Defs.WellKnown;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MoreInjuries.HealthConditions.Choking;

public sealed class HediffComp_Choking : HediffComp
{
    private Std::WeakReference<Hediff_Injury>? _source;

    public HediffCompProperties_Choking Properties => (HediffCompProperties_Choking)props;

    public override void CompPostMake()
    {
        if (MoreInjuriesMod.Settings.EnableChokingSounds)
        {
            KnownSoundDefOf.Choking.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
        }
    }

    public Hediff_Injury? Source
    {
        get
        {
            if (_source is null || !_source.TryGetTarget(out Hediff_Injury? source))
            {
                return null;
            }
            return source;
        }
        set
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
    }

    private bool Coughing => 
        Source is not { Bleeding: true } && (parent.pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness) > 0.3f
        || ModLister.BiotechInstalled && parent.pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest));

    public override string CompLabelInBracketsExtra => Coughing
        ? "MI_Coughing".Translate()
        : string.Empty;

    public override void CompExposeData()
    {
        Hediff_Injury? source = Source;
        Scribe_References.Look(ref source, "source");
        Source = source;
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (!parent.pawn.IsHashIntervalTick(Properties.ChokingIntervalTicks))
        {
            return;
        }
        Hediff_Injury? source = Source;
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
        bool coughing = Coughing;
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
                SoundDef soundDef = (coughing, parent.pawn.gender) switch
                {
                    (true, Gender.Female) => KnownSoundDefOf.ChokingCoughFemale,
                    (true, _) => KnownSoundDefOf.ChokingCoughMale,
                    _ => KnownSoundDefOf.Choking,
                };
                soundDef.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
            }
        }
        else
        {
            parent.pawn.health.RemoveHediff(parent);
        }
    }
}
