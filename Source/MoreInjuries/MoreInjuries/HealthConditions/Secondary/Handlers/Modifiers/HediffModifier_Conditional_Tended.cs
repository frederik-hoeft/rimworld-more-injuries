using MoreInjuries.Debug;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public sealed partial class HediffModifier_Conditional_Tended : SecondaryHediffModifier
{
    [XmlBinding("onTrue")]
    public partial SecondaryHediffModifier? OnTrue { get; }

    [XmlBinding("onFalse")]
    public partial SecondaryHediffModifier? OnFalse { get; }

    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        DebugAssert.IsTrue(OnTrue != null || OnFalse != null, "At least one of OnTrue or OnFalse must be defined.");
        SecondaryHediffModifier? modifier = hediff.IsTended() ? OnTrue : OnFalse;
        return modifier?.GetModifier(hediff, compHandler) ?? Unchanged;
    }
}
