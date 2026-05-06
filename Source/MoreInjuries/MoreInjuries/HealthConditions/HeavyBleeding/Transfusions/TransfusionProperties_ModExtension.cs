using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

[XmlSerializable]
public sealed partial class TransfusionProperties_ModExtension : DefModExtension
{
    [XmlMember("bloodLossSeverityReduction")]
    public partial float BloodLossSeverityReduction { get; }
}
