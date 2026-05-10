using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

[XmlBindable]
public sealed partial class BleedRateModifier_Severity : BleedRateModifier
{
    [XmlBinding<float>("offset", defaultValue: 0f)]
    public partial float Offset { get; }

    public override float GetModifierFor(Hediff hediff, HediffWithComps bleedingHediff) => hediff.Severity + Offset;
}
