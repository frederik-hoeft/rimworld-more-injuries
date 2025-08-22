using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public abstract class TreatmentModifier
{
    // do not rename these fields. XML defs depend on these names
    private readonly JobDef jobDef = default!;

    public JobDef JobDef => jobDef;

    public abstract float GetEffectiveness(Hediff hediff);
}
