using MoreInjuries.Defs;
using System.Collections.Generic;
using System.Text;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_DynamicRuntime_ProcedureCall() : FloatOperator_DynamicRuntime_ProcedureBase
{
    private List<FloatOperator>? _instructions = null;

    // don't rename this field. XML defs depend on this name
    public readonly List<FloatOperator_Assign>? parameters = default;
    // don't rename this field. XML defs depend on this name
    public readonly ReferenceableDef procedureDef = default!;

    internal FloatOperator_DynamicRuntime_ProcedureCall(ReferenceableDef procedureDef, List<FloatOperator_Assign>? parameters) : this()
    {
        this.parameters = parameters;
        this.procedureDef = procedureDef;
    }

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

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append("call ").Append(procedureDef.defName.Trim()).AppendLine(":");
        List<FloatOperator> instructions = LoadInstructions();
        sb.AppendLine("  instructions:");
        for (int i = 0; i < instructions.Count; i++)
        {
            sb.Append("    ").Append(i + 1).Append(": ").AppendLine(instructions[i].ToString());
        }
        return sb.ToString();
    }
}
