using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

[XmlBindable]
public sealed partial class TransfusionProperties_ModExtension : DefModExtension
{
    [XmlBinding("bloodLossSeverityReduction")]
    public partial float BloodLossSeverityReduction { get; }
}
