using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.HealthConditions.Choking;
using MoreInjuries.Things;
using Verse;
using Verse.AI;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_ManageAirways : WorkGiver_MoreInjuriesTreatmentBase
{
    public override bool ShouldSkip(Pawn pawn, bool forced = false) => !KnownResearchProjectDefOf.Cpr.IsFinished;

    protected override bool CanTreat(Hediff hediff) => JobDriver_UseSuctionDevice.JobCanTreat(hediff);

    protected override Job CreateJob(Pawn doctor, Pawn patient) =>
        KnownResearchProjectDefOf.EmergencyMedicine.IsFinished && MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.SuctionDevice, JobDriver_UseSuctionDevice.JobCanTreat) is Thing suctionDevice
        ? JobDriver_UseSuctionDevice.GetDispatcher(doctor, patient, suctionDevice).CreateJob()
        : JobDriver_PerformCpr.GetDispatcher(doctor, patient).CreateJob();
}
