using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.Things.Modifiers;

[XmlBindable]
public sealed partial class ThingModifier_ConstantFactor : ThingModifier
{
    [XmlBinding("factor", NullableBackingField = true)]
    public partial float Factor { get; }

    public override float GetModifier(Thing thing) => Factor;
}
