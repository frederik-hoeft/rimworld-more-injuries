using MoreInjuries.BuildIntrinsics;
using MoreInjuries.HealthConditions.MechaniteTherapy;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.BrainDamage;

// members initialized via XML defs
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class HediffCompProperties_MechaniteTherapy : HediffCompProperties
{
    // do not rename this field. XML defs depend on this name
    private readonly float daysToComplete = default;

    public float InitialSeverity => Mathf.Max(0.1f, daysToComplete);

    public HediffCompProperties_MechaniteTherapy() => compClass = typeof(HediffComp_MechaniteTherapy);
}
