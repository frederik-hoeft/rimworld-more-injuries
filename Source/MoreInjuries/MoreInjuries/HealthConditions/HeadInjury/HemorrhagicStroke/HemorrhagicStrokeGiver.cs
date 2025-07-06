using MoreInjuries.Defs.WellKnown;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeadInjury.HemorrhagicStroke;

public class HemorrhagicStrokeGiver : HeadInjuryGiver
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableHemorrhagicStroke;

    protected override float SettingsMaximumEquivalentDamageSkull => MoreInjuriesMod.Settings.HemorrhagicStrokeThreshold;

    protected override float SettingsChance => MoreInjuriesMod.Settings.HemorrhagicStrokeChance;

    protected override HediffDef HediffDef => KnownHediffDefOf.HemorrhagicStroke;

    protected override float CalculateSeverity(float equivalentLikeliness)
    {
        // we want an initial severity distribution that is skewed towards the lower end
        float equivalentLikelinessClamped = Mathf.Clamp(equivalentLikeliness, 0.01f, 1.0f);
        float severityFactor = Rand.Range(0.01f, equivalentLikelinessClamped);
        return severityFactor * severityFactor;
    }
}
