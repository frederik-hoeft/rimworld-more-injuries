using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Injectors.Outcomes.Conditions;

public sealed class OutcomeDoerCondition_Or : OutcomeDoerCondition_Composite
{
    public override bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        IReadOnlyList<OutcomeDoerCondition> conditions = Conditions;
        if (conditions is not { Count: > 0 })
        {
            Logger.Error($"{nameof(OutcomeDoerCondition_Or)} has no conditions defined");
            return false;
        }
        foreach (OutcomeDoerCondition condition in conditions)
        {
            if (condition.ShouldDoOutcome(doctor, patient, device))
            {
                return true;
            }
        }
        return false;
    }
}