using System.Collections.Generic;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_DynamicRuntime : FloatOperator_DynamicRuntime_ProcedureBase
{
    // don't rename this field. XML defs depend on this name
    public readonly List<FloatOperator> instructions = default!;

    protected override List<FloatOperator> LoadInstructions() => instructions;
}
