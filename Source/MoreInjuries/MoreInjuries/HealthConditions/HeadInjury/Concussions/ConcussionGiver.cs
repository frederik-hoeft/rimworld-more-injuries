using MoreInjuries.KnownDefs;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeadInjury.Concussions;

public class ConcussionGiver() : HeadInjuryGiver()
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableConcussion;

    protected override float SettingsMaximumEquivalentDamageSkull => MoreInjuriesMod.Settings.ConcussionThreshold;

    protected override float SettingsChance => MoreInjuriesMod.Settings.ConcussionChance;

    protected override HediffDef HediffDef => KnownHediffDefOf.Concussion;

    protected override float CalculateSeverity(float equivalentLikeliness)
    {
        // we want a severity distribution that scales with the severity of the injury
        // using the unclamped equivalent likeliness as a base, allowing the linear random distribution to be shifted above 1.0
        float severityFactor = Rand.Range(0f, equivalentLikeliness);
        // ... and finally clamping the result to ensure that the severity is within the expected range,
        // allowing 1.0 to be more common if the equivalent likeliness is above 1.0
        return Mathf.Clamp01(severityFactor);
    }
}
