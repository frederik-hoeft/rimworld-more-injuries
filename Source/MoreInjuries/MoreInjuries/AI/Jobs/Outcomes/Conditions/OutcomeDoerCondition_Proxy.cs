using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions;

[XmlBindable]
public abstract partial class OutcomeDoerCondition_Proxy : OutcomeDoerCondition 
{
    [XmlBinding("condition")]
    public partial OutcomeDoerCondition Condition { get; }
}