using Verse;

namespace MoreInjuries.HealthConditions.Injectors.Outcomes.Conditions;

public abstract class OutcomeDoerCondition
{
    public abstract bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device);
}
