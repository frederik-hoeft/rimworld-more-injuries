using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding;

[XmlSerializable]
public partial class HemostasisModExtension : DefModExtension
{
    [XmlMember("coagulationMultiplier")]
    public partial float CoagulationMultiplier { get; }

    [XmlMember("disappearsAfterTicks")]
    public partial int DisappearsAfterTicks { get; }
}
