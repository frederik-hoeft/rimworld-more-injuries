using Verse.AI;
using Verse;
using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.KnownDefs;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_PerformCpr : WorkGiver_MoreInjuriesTreatmentBase
{
    public override bool ShouldSkip(Pawn pawn, bool forced = false) => !KnownResearchProjectDefOf.EmergencyMedicine.IsFinished;

    protected override bool CanTreat(Hediff hediff) => Array.IndexOf(JobDriver_PerformCpr.TargetHediffDefs, hediff.def) != -1;

    protected override Job CreateJob(Pawn doctor, Pawn patient) => JobDriver_PerformCpr.GetDispatcher(doctor, patient).CreateJob();
}
