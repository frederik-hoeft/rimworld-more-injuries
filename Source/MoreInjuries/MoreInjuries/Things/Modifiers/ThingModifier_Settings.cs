using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

namespace MoreInjuries.Things.Modifiers;

[XmlSerializable]
public abstract partial class ThingModifier_Settings : ThingModifier
{
    [XmlMember("key")]
    public partial string Key { get; }
}
