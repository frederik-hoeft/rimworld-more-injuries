using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using System.Text;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

[XmlSerializable]
public sealed partial class FloatOperator_DynamicRuntime : FloatOperator_DynamicRuntime_ProcedureBase
{
    [XmlMember("instructions")]
    public partial List<FloatOperator> Instructions { get; }

    protected override List<FloatOperator> LoadInstructions() => Instructions;

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
