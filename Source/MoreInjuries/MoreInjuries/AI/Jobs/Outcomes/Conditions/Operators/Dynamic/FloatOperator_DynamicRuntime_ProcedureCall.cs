using MoreInjuries.Defs;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using System.Text;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

[XmlBindable]
public sealed partial class FloatOperator_DynamicRuntime_ProcedureCall() : FloatOperator_DynamicRuntime_ProcedureBase
{
    private List<FloatOperator>? _instructions = null;

    [XmlBinding("parameters")]
    public partial List<FloatOperator_Assign>? Parameters { get; init; }

    [XmlBinding("procedureDef")]
    public partial ReferenceableDef ProcedureDef { get; init; }

    internal FloatOperator_DynamicRuntime_ProcedureCall(ReferenceableDef procedureDef, List<FloatOperator_Assign>? parameters) : this()
    {
        Parameters = parameters;
        ProcedureDef = procedureDef;
    }

    protected override List<FloatOperator> LoadInstructions()
    {
        if (_instructions is not null)
        {
            return _instructions;
        }
        if (ProcedureDef.GetModExtension<DynamicRuntimeProcedureDef_ModExtension>() is not { Instructions: { Count: > 0 } instructions })
        {
            throw new InvalidOperationException($"{nameof(FloatOperator_DynamicRuntime_ProcedureCall)}: procedureDef '{ProcedureDef.defName}' contains no instructions");
        }
        return _instructions = Parameters switch
        {
            { Count: > 0 } => [.. Parameters, .. instructions],
            _ => instructions
        };
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append("call ").Append(ProcedureDef.defName.Trim()).AppendLine(":");
        List<FloatOperator> instructions = LoadInstructions();
        sb.AppendLine("  instructions:");
        for (int i = 0; i < instructions.Count; i++)
        {
            sb.Append("    ").Append(i + 1).Append(": ").AppendLine(instructions[i].ToString());
        }
        return sb.ToString();
    }
}
