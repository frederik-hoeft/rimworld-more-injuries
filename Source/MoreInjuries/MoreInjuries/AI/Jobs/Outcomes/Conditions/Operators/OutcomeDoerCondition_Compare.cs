using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Comparison;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;

[XmlSerializable]
public sealed partial class OutcomeDoerCondition_Compare : OutcomeDoerCondition
{
    [XmlMember("comparisonOperator")]
    public partial ComparisonOperator ComparisonOperator { get; }

    [XmlMember("left")]
    public partial FloatOperator Left { get; init; }

    [XmlMember("right")]
    public partial FloatOperator Right { get; init; }

    public override bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        float leftValue = Left.Evaluate(doctor, patient, device, runtimeState);
        float rightValue = Right.Evaluate(doctor, patient, device, runtimeState);
        return ComparisonOperator.Compare(leftValue, rightValue);
    }

    public override string ToString() => $"({Left} {ComparisonOperator} {Right})";
}
