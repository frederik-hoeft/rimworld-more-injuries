using System.Collections.Generic;
using System.Text;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public abstract class OutcomeDoerCondition_Composite : OutcomeDoerCondition
{
    // don't rename this field. XML defs depend on this name
    private readonly List<OutcomeDoerCondition> conditions = default!;

    public IReadOnlyList<OutcomeDoerCondition> Conditions => conditions;

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