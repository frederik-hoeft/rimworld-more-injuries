using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[XmlSerializable]
public partial class WeightedHediffMakerDef : HediffMakerDef
{
    [XmlMember<float>("weight", defaultValue: 1f)]
    public partial float Weight { get; }
}
