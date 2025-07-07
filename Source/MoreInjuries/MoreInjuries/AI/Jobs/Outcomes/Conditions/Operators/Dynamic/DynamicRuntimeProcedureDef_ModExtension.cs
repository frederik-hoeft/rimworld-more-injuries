using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
public sealed class DynamicRuntimeProcedureDef_ModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    public readonly List<FloatOperator>? instructions = default!;

    public List<FloatOperator>? Instructions => instructions;
}
