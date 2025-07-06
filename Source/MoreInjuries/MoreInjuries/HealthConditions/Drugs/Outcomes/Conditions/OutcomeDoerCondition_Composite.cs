using MoreInjuries.BuildIntrinsics;
using System.Collections.Generic;

namespace MoreInjuries.HealthConditions.Drugs.Outcomes.Conditions;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public abstract class OutcomeDoerCondition_Composite : OutcomeDoerCondition
{
    // don't rename this field. XML defs depend on this name
    private readonly List<OutcomeDoerCondition> conditions = default!;

    public IReadOnlyList<OutcomeDoerCondition> Conditions => conditions;
}