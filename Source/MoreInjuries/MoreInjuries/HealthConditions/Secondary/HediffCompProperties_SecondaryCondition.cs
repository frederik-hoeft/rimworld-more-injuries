using MoreInjuries.HealthConditions.Secondary.Handlers;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffCompProperties_SecondaryCondition : HediffCompProperties
{
    private static readonly IReadOnlyList<HediffCompHandler_SecondaryCondition> s_emptyHandlers = [];

    // don't rename this field. XML defs depend on this name
    private readonly SimpleCurve? severityCurve = default;
    // don't rename this field. XML defs depend on this name
    private readonly List<HediffCompHandler_SecondaryCondition>? handlers = null;

    public HediffCompProperties_SecondaryCondition() => compClass = typeof(HediffComp_SecondaryCondition);

    public SimpleCurve? SeverityCurve => severityCurve;

    public IReadOnlyList<HediffCompHandler_SecondaryCondition> Handlers => handlers ?? s_emptyHandlers;
}