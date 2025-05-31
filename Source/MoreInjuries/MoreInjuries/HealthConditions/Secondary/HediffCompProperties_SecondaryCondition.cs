using MoreInjuries.BuildIntrinsics;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffCompProperties_SecondaryCondition : HediffCompProperties
{
    private static readonly IReadOnlyList<HediffCompHandler_SecondaryCondition> s_emptyHandlers = [];

    // don't rename this field. XML defs depend on this name
    private readonly SimpleCurve severityCurve = default!;
    // don't rename this field. XML defs depend on this name
    private readonly List<HediffCompHandler_SecondaryCondition>? handlers = null;

    public HediffCompProperties_SecondaryCondition() => compClass = typeof(HediffComp_SecondaryCondition);

    public SimpleCurve SeverityCurve => severityCurve;

    public IReadOnlyList<HediffCompHandler_SecondaryCondition> Handlers => handlers ?? s_emptyHandlers;
}