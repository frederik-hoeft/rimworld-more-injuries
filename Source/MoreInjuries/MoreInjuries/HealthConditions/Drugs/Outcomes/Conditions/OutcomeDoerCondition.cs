using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Outcomes.Conditions;

public abstract class OutcomeDoerCondition
{
    public abstract bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device);
}
