using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Comparison;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;

[XmlBindable]
public sealed partial class OutcomeDoerCondition_Compare : OutcomeDoerCondition
{
    [XmlBinding("comparisonOperator")]
    public partial ComparisonOperator ComparisonOperator { get; }

    [XmlBinding("left")]
    public partial FloatOperator Left { get; init; }

    [XmlBinding("right")]
    public partial FloatOperator Right { get; init; }

    public override bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        float leftValue = Left.Evaluate(doctor, patient, device, runtimeState);
        float rightValue = Right.Evaluate(doctor, patient, device, runtimeState);
        return ComparisonOperator.Compare(leftValue, rightValue);
    }

    public override string ToString() => $"({Left} {ComparisonOperator} {Right})";
}
