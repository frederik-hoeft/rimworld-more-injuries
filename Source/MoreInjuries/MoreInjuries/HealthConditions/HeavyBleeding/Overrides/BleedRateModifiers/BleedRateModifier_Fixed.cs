using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

[XmlBindable]
public sealed partial class BleedRateModifier_Fixed : BleedRateModifier
{
    [XmlBinding<float>("factor", defaultValue: 1f)]
    public partial float Factor { get; }

    public override float GetModifierFor(Hediff hediff, HediffWithComps bleedingHediff) => Factor;
}
