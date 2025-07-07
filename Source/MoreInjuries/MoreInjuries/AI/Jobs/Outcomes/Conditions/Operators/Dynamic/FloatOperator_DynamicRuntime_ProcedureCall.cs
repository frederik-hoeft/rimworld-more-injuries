using MoreInjuries.Defs;
using System.Collections.Generic;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_DynamicRuntime_ProcedureCall : FloatOperator_DynamicRuntime_ProcedureBase
{
    private List<FloatOperator>? _instructions = null;

    // don't rename this field. XML defs depend on this name
    public readonly List<FloatOperator_Assign>? parameters = default;
    // don't rename this field. XML defs depend on this name
    public readonly ReferenceableDef procedureDef = default!;

    protected override List<FloatOperator> LoadInstructions()
    {
        if (_instructions is not null)
        {
            return _instructions;
        }
        _ = procedureDef ?? throw new InvalidOperationException($"{nameof(FloatOperator_DynamicRuntime_ProcedureCall)}: procedureDef cannot be null");
        if (procedureDef.GetModExtension<DynamicRuntimeProcedureDef_ModExtension>() is not { Instructions: { Count: > 0 } instructions })
        {
            throw new InvalidOperationException($"{nameof(FloatOperator_DynamicRuntime_ProcedureCall)}: procedureDef '{procedureDef.defName}' contains no instructions");
        }
        return _instructions = parameters switch
        {
            { Count: > 0 } => [.. parameters, .. instructions],
            _ => instructions
        };
    }
}
