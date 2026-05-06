using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

[XmlSerializable]
public sealed partial class BleedRateModifier_ModExtension : DefModExtension
{
    [XmlMember("modifier")]
    public partial BleedRateModifier Modifier { get; }
}
