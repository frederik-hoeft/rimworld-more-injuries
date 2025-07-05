using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class TreatmentModifier_LinkedHediff_SimpleCurve : TreatmentModifier_LinkedHediff
{
    // do not rename these fields. XML defs depend on these names
    private readonly SimpleCurve effectivenessCurve = default!;

    public SimpleCurve EffectivenessCurve => effectivenessCurve;

    protected override float GetEffectiveness(Hediff hediff, Hediff otherHediff)
    {
        return EffectivenessCurve.Evaluate(otherHediff.Severity);
    }
}
