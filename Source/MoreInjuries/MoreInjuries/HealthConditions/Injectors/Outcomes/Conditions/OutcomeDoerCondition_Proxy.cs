using MoreInjuries.BuildIntrinsics;
using System.Diagnostics.CodeAnalysis;

namespace MoreInjuries.HealthConditions.Injectors.Outcomes.Conditions;

[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public abstract class OutcomeDoerCondition_Proxy : OutcomeDoerCondition 
{
    // don't rename this field. XML defs depend on this name
    private readonly OutcomeDoerCondition condition = default!;

    public OutcomeDoerCondition Condition => condition;
}