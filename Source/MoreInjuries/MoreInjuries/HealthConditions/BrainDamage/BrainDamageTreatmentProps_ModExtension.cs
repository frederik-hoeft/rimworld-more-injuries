using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.BrainDamage;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class BrainDamageTreatmentProps_ModExtension : DefModExtension
{
    // do not rename this field. XML defs depend on this name
    private readonly FloatRange severityReductionRange = FloatRange.Zero;
    // do not rename this field. XML defs depend on this name
    private readonly float daysToComplete = default;

    public FloatRange SeverityReductionRange => severityReductionRange;

    public float DaysToComplete => Mathf.Max(0.1f, daysToComplete);
}