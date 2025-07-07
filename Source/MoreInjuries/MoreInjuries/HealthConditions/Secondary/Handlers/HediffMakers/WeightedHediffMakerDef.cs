using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class WeightedHediffMakerDef(HediffDef hediffDef, float? minSeverity, float? maxSeverity, bool? allowDuplicate, bool? allowMultiple, float weight) 
    : HediffMakerDef(hediffDef, minSeverity, maxSeverity, allowDuplicate, allowMultiple)
{
    // don't rename this field. XML defs depend on this name
    private readonly float weight = weight;

    public WeightedHediffMakerDef() : this(default!, null, null, null, null, 1f) { }

    public float Weight => weight;
}
