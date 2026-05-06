using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlSerializable]
public partial class HediffModifier_ConstantFactor : SecondaryHediffModifier
{
    [XmlMember("factor", NullableBackingField = true)]
    public partial float Factor { get; }

    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler) => Factor;
}
