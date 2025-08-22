using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_Settings_FeatureFlag_Disabled : HediffModifier_Settings_FeatureFlag
{
    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler) => 1f - base.GetModifier(hediff, compHandler);
}