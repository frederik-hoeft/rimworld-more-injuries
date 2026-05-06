using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

[XmlBindable]
public sealed partial class BleedRateModifier_SimpleCurve : BleedRateModifier
{
    [XmlBinding("severityCurve")]
    public partial SimpleCurve SeverityCurve { get; }

    public override float GetModifierFor(Hediff hediff, HediffWithComps bleedingHediff) => SeverityCurve.Evaluate(hediff.Severity);
}
