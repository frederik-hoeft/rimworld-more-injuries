using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;

namespace MoreInjuries.Things.Modifiers;

[XmlBindable]
public abstract partial class ThingModifier_Settings : ThingModifier
{
    [XmlBinding("key")]
    public partial string Key { get; }
}
