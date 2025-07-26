namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Comparison;

public sealed class ComparisonOperator_LessThan : ComparisonOperator
{
    public override bool Compare(float left, float right) => left < right;

    public override string ToString() => "<";
}
