namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Comparison;

public sealed class ComparisonOperator_LessThanOrEqual : ComparisonOperator
{
    public override bool Compare(float left, float right) => left <= right;
}