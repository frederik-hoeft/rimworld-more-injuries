using Verse;

namespace MoreInjuries.HealthConditions.Injectors.Outcomes.Conditions;

public class OutcomeDoerCondition_RequiresMedicalAttention : OutcomeDoerCondition
{
    public override bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device) => patient.health?.hediffSet.hediffs is { Count: > 0 } 
        && patient.health.hediffSet.hediffs.Any(static hediff => hediff.TendableNow() || hediff.IsAnyStageLifeThreatening());
}
