using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

[XmlBindable]
public partial class HediffCompProperties_Shock : HediffCompProperties<HediffComp_Shock>
{
    // TODO: [BREAKING] rename to standardized format (no _ prefix) for consistency
    [XmlBinding("_bleedSeverityCurve")]
    public partial SimpleCurve BleedSeverityCurve { get; }
}
