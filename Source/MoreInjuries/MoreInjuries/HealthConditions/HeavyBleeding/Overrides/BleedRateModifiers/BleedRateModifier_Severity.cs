using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

[XmlSerializable]
public sealed partial class BleedRateModifier_Severity : BleedRateModifier
{
    [XmlMember<float>("offset", defaultValue: 0f)]
    public partial float Offset { get; }

    public override float GetModifierFor(Hediff hediff, HediffWithComps bleedingHediff) => hediff.Severity + Offset;
}
