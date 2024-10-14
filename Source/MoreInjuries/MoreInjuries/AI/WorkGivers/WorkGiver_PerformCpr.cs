using Verse.AI;
using Verse;
using MoreInjuries.HealthConditions.CardiacArrest;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_PerformCpr : WorkGiver_MoreInjuriesTreatmentBase
{
    protected override bool CanTreat(Hediff hediff) => Array.IndexOf(JobDriver_PerformCpr.TargetHediffDefs, hediff.def) != -1;

    protected override Job CreateJob(Pawn doctor, Pawn patient) => JobDriver_PerformCpr.GetDispatcher(doctor, patient).CreateJob();
}
