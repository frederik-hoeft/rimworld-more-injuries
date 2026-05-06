using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

[XmlSerializable]
public partial class HediffCompProperties_Shock : HediffCompProperties
{
    public HediffCompProperties_Shock() => compClass = typeof(HediffComp_Shock);

    // TODO: [BREAKING] rename to standardized format (no _ prefix) for consistency
    [XmlMember("_bleedSeverityCurve")]
    public partial SimpleCurve BleedSeverityCurve { get; }
}
