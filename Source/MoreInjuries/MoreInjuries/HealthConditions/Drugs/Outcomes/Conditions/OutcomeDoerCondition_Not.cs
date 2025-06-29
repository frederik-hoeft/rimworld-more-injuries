using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Outcomes.Conditions;

public sealed class OutcomeDoerCondition_Not : OutcomeDoerCondition_Proxy
{
    public override bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        return !Condition.ShouldDoOutcome(doctor, patient, device);
    }
}