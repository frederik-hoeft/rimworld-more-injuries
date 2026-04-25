using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions;

[XmlSerializable]
public abstract partial class OutcomeDoerCondition_Proxy : OutcomeDoerCondition 
{
    [XmlMember("condition")]
    public partial OutcomeDoerCondition Condition { get; }
}