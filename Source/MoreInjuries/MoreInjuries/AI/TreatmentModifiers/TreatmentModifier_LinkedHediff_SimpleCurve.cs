using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
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
