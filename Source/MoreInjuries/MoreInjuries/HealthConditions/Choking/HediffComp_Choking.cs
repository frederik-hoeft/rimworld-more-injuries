using MoreInjuries.Defs.WellKnown;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MoreInjuries.HealthConditions.Choking;

public class HediffComp_Choking : HediffComp
{
    private Hediff_Injury? _source;

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
        get => _source;
        set => _source = value;
    }

    private bool Coughing => 
        Source is { Bleeding: false } && (parent.pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness) > 0.45f
        || ModLister.BiotechInstalled && parent.pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest));

    public override string CompLabelInBracketsExtra => Coughing
        ? "MI_Coughing".Translate()
        : string.Empty;

    public override void CompExposeData() => Scribe_References.Look(ref _source, "source");

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (!parent.pawn.IsHashIntervalTick(Properties.ChokingIntervalTicks))
        {
            return;
        }
        if (Source is null)
        {
            Logger.Warning("Choking hediff has no source injury! Was this hediff added manually?");
            return;
        }
        // a random walk with a bias towards increasing severity, increase depends on the bleed rate of the source injury and whether the patient is tended
        float increase = 0.1f;
        float decrease = 0f;
        if (Source.BleedRate > 0.01f)
        {
            increase += Mathf.Clamp(Source.BleedRate / 5f, 0.05f, 0.25f);
        }
        else if (Source.IsTended())
        {
            decrease = 0.075f;
        }
        else if (Source.BleedRate <= 0.01f)
        {
            decrease = 0.05f;
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
        if (newSeverity > 0f)
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
