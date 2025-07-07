using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public abstract class TreatmentModifier_LinkedHediff : TreatmentModifier
{
    // do not rename these fields. XML defs depend on these names
    private readonly HediffDef otherHediffDef = default!;

    public HediffDef OtherHediffDef => otherHediffDef;

    protected abstract float GetEffectiveness(Hediff hediff, Hediff otherHediff);

    public override float GetEffectiveness(Hediff hediff)
    {
        if (hediff.pawn.health.hediffSet.TryGetHediff(OtherHediffDef, out Hediff? otherHediff))
        {
            return GetEffectiveness(hediff, otherHediff);
        }
        return 1f;
    }
}
