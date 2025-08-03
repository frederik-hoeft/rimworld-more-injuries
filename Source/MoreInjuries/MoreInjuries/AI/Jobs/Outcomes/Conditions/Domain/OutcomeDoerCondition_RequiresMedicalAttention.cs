using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Domain;

public sealed class OutcomeDoerCondition_RequiresMedicalAttention : OutcomeDoerCondition
{
    public override bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) => 
        patient.health?.hediffSet.hediffs is { Count: > 0 } 
        && patient.health.hediffSet.hediffs.Any(static hediff => hediff.TendableNow() || hediff.IsAnyStageLifeThreatening());

    public override string ToString() => "requires_medical_attention()";
}