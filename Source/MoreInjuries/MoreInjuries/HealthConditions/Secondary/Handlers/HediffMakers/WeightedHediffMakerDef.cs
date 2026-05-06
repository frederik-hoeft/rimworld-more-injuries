using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[XmlBindable]
public partial class WeightedHediffMakerDef : HediffMakerDef
{
    [XmlBinding<float>("weight", defaultValue: 1f)]
    public partial float Weight { get; }
}
