using System.Collections.Generic;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public abstract class OutcomeDoerCondition_Composite : OutcomeDoerCondition
{
    // don't rename this field. XML defs depend on this name
    private readonly List<OutcomeDoerCondition> conditions = default!;

    public IReadOnlyList<OutcomeDoerCondition> Conditions => conditions;
}