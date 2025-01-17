using Verse.AI;
using Verse;
using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.Things;
using MoreInjuries.KnownDefs;
using MoreInjuries.HealthConditions.Choking;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_ManageAirways : WorkGiver_MoreInjuriesTreatmentBase
{
    public override bool ShouldSkip(Pawn pawn, bool forced = false) => !KnownResearchProjectDefOf.EmergencyMedicine.IsFinished;

    protected override bool CanTreat(Hediff hediff) => Array.IndexOf(JobDriver_UseSuctionDevice.TargetHediffDefs, hediff.def) != -1;

    protected override Job CreateJob(Pawn doctor, Pawn patient) => MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.SuctionDevice, JobDriver_UseSuctionDevice.TargetHediffDefs) is Thing suctionDevice
        ? JobDriver_UseSuctionDevice.GetDispatcher(doctor, patient, suctionDevice).CreateJob()
        : JobDriver_PerformCpr.GetDispatcher(doctor, patient).CreateJob();
}
