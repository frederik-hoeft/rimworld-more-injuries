using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding;

[XmlBindable]
public partial class HemostasisModExtension : DefModExtension
{
    [XmlBinding("coagulationMultiplier")]
    public partial float CoagulationMultiplier { get; }

    [XmlBinding("disappearsAfterTicks")]
    public partial int DisappearsAfterTicks { get; }
}
