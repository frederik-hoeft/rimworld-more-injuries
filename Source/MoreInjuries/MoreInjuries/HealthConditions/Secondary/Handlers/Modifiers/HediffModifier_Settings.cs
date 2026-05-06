using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlSerializable]
public abstract partial class HediffModifier_Settings : SecondaryHediffModifier
{
    [XmlMember("key")]
    public partial string Key { get; }
}
