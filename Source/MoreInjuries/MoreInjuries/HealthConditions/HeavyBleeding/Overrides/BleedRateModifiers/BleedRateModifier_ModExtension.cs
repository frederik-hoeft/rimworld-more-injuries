using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

[XmlBindable]
public sealed partial class BleedRateModifier_ModExtension : DefModExtension
{
    [XmlBinding("modifier")]
    public partial BleedRateModifier Modifier { get; }
}
