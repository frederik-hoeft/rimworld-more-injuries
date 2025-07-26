using UnityEngine;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Comparison;

public sealed class ComparisonOperator_Equals : ComparisonOperator
{
    public override bool Compare(float left, float right) => Mathf.Approximately(left, right);

    public override string ToString() => "==";
}
