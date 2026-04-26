using Verse;

namespace MoreInjuries.Things.Modifiers;

public sealed class ThingModifier_Settings_FeatureFlag_Disabled : ThingModifier_Settings_FeatureFlag
{
    public override float GetModifier(Thing thing) => 1f - base.GetModifier(thing);
}
