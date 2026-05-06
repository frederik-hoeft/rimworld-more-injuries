using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

[XmlSerializable]
public sealed partial class TreatmentModifier_LinkedHediff_SimpleCurve : TreatmentModifier_LinkedHediff
{
    [XmlMember("effectivenessCurve")]
    public partial SimpleCurve EffectivenessCurve { get; }

    protected override float GetEffectiveness(Hediff hediff, Hediff otherHediff) =>
        EffectivenessCurve.Evaluate(otherHediff.Severity);
}
