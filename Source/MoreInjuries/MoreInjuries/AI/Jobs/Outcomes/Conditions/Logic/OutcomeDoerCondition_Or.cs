using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Logic;

public sealed class OutcomeDoerCondition_Or : OutcomeDoerCondition_Composite
{
    protected override string OperatorName => "or";

    public override bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        IReadOnlyList<OutcomeDoerCondition> conditions = Conditions;
        if (conditions is not { Count: > 0 })
        {
            Logger.Error($"{nameof(OutcomeDoerCondition_Or)} has no conditions defined");
            return false;
        }
        foreach (OutcomeDoerCondition condition in conditions)
        {
            if (condition.ShouldDoOutcome(doctor, patient, device, runtimeState))
            {
                return true;
            }
        }
        return false;
    }
}