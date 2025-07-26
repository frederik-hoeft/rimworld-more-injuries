using System.Collections.Generic;
using System.Text;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_DynamicRuntime : FloatOperator_DynamicRuntime_ProcedureBase
{
    // don't rename this field. XML defs depend on this name
    public readonly List<FloatOperator> instructions = default!;

    protected override List<FloatOperator> LoadInstructions() => instructions;

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine("eval:");
        List<FloatOperator> instructions = LoadInstructions();
        sb.AppendLine("  instructions:");
        for (int i = 0; i < instructions.Count; i++)
        {
            sb.Append("    ").Append(i + 1).Append(": ").AppendLine(instructions[i].ToString());
        }
        return sb.ToString();
    }
}
