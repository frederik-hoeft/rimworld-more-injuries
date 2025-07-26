using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;
using System.Text;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class JobOutcomeDoer_HediffOffset_Dynamic : JobOutcomeDoer_HediffOffsetBase
{
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator? evaluator = default;

    protected override float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device)
    {
        if (evaluator is null)
        {
            Logger.ConfigError($"{GetType().Name}: Missing or invalid evaluator. Severity offset cannot be calculated.");
            return 0f;
        }
        // evaluate the expression using the doctor, patient, and device as context
        return evaluator.Evaluate(doctor, patient, device, runtimeState: null);
    }

    public override string ToString() => $"{base.ToString()} with dynamic severity offset evaluator: {evaluator?.ToString() ?? "null"}";
}