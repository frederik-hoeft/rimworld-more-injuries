using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Comparison;

[XmlBindable]
public sealed partial class ComparisonOperator_Not : ComparisonOperator
{
    [XmlBinding("inner")]
    public partial ComparisonOperator Inner { get; }

    public override bool Compare(float left, float right) => !Inner.Compare(left, right);

    public override string ToString() => $"not({Inner})";
}
