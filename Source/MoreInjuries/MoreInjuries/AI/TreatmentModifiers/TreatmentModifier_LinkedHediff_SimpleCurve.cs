using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

[XmlBindable]
public sealed partial class TreatmentModifier_LinkedHediff_SimpleCurve : TreatmentModifier_LinkedHediff
{
    [XmlBinding("effectivenessCurve")]
    public partial SimpleCurve EffectivenessCurve { get; }

    protected override float GetEffectiveness(Hediff hediff, Hediff otherHediff) =>
        EffectivenessCurve.Evaluate(otherHediff.Severity);
}
