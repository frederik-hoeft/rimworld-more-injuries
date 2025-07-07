using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions;

public abstract class OutcomeDoerCondition
{
    public abstract bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState);
}
