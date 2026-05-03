using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using System.Text;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions;

[XmlSerializable]
public abstract partial class OutcomeDoerCondition_Composite : OutcomeDoerCondition
{
    [XmlMember("conditions")]
    public partial IReadOnlyList<OutcomeDoerCondition> Conditions { get; }

    protected abstract string OperatorName { get; }

    public override string ToString()
    {
        StringBuilder sb = new('(');
        foreach (OutcomeDoerCondition condition in Conditions)
        {
            if (sb.Length > 0)
            {
                sb.Append(' ').Append(OperatorName).Append(' ');
            }
            sb.Append(condition.ToString());
        }
        sb.Append(')');
        return sb.ToString();
    }
}
