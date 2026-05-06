using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

[XmlSerializable]
public sealed partial class BleedRateModifier_Fixed : BleedRateModifier
{
    [XmlMember<float>("factor", defaultValue: 1f)]
    public partial float Factor { get; }

    public override float GetModifierFor(Hediff hediff, HediffWithComps bleedingHediff) => Factor;
}
