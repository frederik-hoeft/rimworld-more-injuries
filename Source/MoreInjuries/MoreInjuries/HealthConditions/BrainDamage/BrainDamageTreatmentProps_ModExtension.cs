using MoreInjuries.BuildIntrinsics;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.BrainDamage;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class BrainDamageTreatmentProps_ModExtension : DefModExtension
{
    // do not rename this field. XML defs depend on this name
    private readonly FloatRange severityReductionRange = FloatRange.Zero;
    // do not rename this field. XML defs depend on this name
    private readonly float daysToComplete = default;

    public FloatRange SeverityReductionRange => severityReductionRange;

    public float DaysToComplete => Mathf.Max(0.1f, daysToComplete);
}