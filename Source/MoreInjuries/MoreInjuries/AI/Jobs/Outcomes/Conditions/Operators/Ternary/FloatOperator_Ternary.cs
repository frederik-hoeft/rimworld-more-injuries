using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Ternary;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_Ternary : FloatOperator
{
    // don't rename this field. XML defs depend on this name
    private readonly OutcomeDoerCondition? condition = default;
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator? whenTrue = default;
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator? whenFalse = default;

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        _ = condition ?? throw new InvalidOperationException($"{nameof(FloatOperator_Ternary)}: {nameof(condition)} is not set. This operator requires a condition to evaluate.");
        _ = whenTrue ?? throw new InvalidOperationException($"{nameof(FloatOperator_Ternary)}: {nameof(whenTrue)} is not set. This operator requires a true case to evaluate.");
        _ = whenFalse ?? throw new InvalidOperationException($"{nameof(FloatOperator_Ternary)}: {nameof(whenFalse)} is not set. This operator requires a false case to evaluate.");
        if (condition.ShouldDoOutcome(doctor, patient, device, runtimeState))
        {
            return whenTrue.Evaluate(doctor, patient, device, runtimeState);
        }
        return whenFalse.Evaluate(doctor, patient, device, runtimeState);
    }

    public override string ToString() => $"({condition} ? {whenTrue} : {whenFalse})";
}
