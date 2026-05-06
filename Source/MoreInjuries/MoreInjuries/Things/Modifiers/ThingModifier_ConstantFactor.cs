using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.Things.Modifiers;

[XmlSerializable]
public sealed partial class ThingModifier_ConstantFactor : ThingModifier
{
    [XmlMember("factor", NullableBackingField = true)]
    public partial float Factor { get; }

    public override float GetModifier(Thing thing) => Factor;
}
