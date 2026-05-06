using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public partial class HediffModifier_ConstantFactor : SecondaryHediffModifier
{
    [XmlBinding("factor", NullableBackingField = true)]
    public partial float Factor { get; }

    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler) => Factor;
}
