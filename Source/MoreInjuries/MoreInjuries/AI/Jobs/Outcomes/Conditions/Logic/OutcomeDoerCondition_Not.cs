using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Logic;

public sealed class OutcomeDoerCondition_Not : OutcomeDoerCondition_Proxy
{
    public override bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) => 
        !Condition.ShouldDoOutcome(doctor, patient, device, runtimeState);
}