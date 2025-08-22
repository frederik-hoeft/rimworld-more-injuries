using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Comparison;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class OutcomeDoerCondition_Compare : OutcomeDoerCondition
{
    // don't rename this field. XML defs depend on this name
    private readonly ComparisonOperator? comparisonOperator = default;
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator? left = default;
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator? right = default;

    public override bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        if (comparisonOperator is null || left is null || right is null)
        {
            Logger.ConfigError($"{GetType().Name}: Missing or invalid comparison operator or operands. Cannot evaluate condition: op: {comparisonOperator?.GetType().Name}, left: {left?.GetType().Name}, right: {right?.GetType().Name}");
            return false;
        }
        float leftValue = left.Evaluate(doctor, patient, device, runtimeState);
        float rightValue = right.Evaluate(doctor, patient, device, runtimeState);
        return comparisonOperator.Compare(leftValue, rightValue);
    }

    public override string ToString() => $"({left} {comparisonOperator} {right})";
}
