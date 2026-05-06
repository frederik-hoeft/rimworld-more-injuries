using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using System.Text;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions;

[XmlBindable]
public abstract partial class OutcomeDoerCondition_Composite : OutcomeDoerCondition
{
    [XmlBinding("conditions")]
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
