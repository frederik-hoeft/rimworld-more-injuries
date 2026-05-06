using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

[XmlSerializable]
public sealed partial class BleedRateModifier_SimpleCurve : BleedRateModifier
{
    [XmlMember("severityCurve")]
    public partial SimpleCurve SeverityCurve { get; }

    public override float GetModifierFor(Hediff hediff, HediffWithComps bleedingHediff) => SeverityCurve.Evaluate(hediff.Severity);
}
