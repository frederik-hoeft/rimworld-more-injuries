using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Comparison;

[XmlSerializable]
public sealed partial class ComparisonOperator_Not : ComparisonOperator
{
    [XmlMember("inner")]
    public partial ComparisonOperator Inner { get; }

    public override bool Compare(float left, float right) => !Inner.Compare(left, right);

    public override string ToString() => $"not({Inner})";
}
