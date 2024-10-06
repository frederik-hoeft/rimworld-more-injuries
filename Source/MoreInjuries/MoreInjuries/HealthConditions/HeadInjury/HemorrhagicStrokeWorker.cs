using MoreInjuries.KnownDefs;
using Verse;

namespace MoreInjuries.HealthConditions.HeadInjury;

public class HemorrhagicStrokeWorker(InjuryComp parent) : HeadInjuryWorker(parent)
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableHemorrhagicStroke;

    protected override float SettingsMaximumEquivalentDamageSkull => MoreInjuriesMod.Settings.HemorrhagicStrokeThreshold;

    protected override float SettingsChance => MoreInjuriesMod.Settings.HemorrhagicStrokeChance;

    protected override HediffDef HediffDef => KnownHediffDefOf.HemorrhagicStroke;
}
