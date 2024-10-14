using Verse.AI;
using Verse;
using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.Things;
using MoreInjuries.KnownDefs;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_ResuscitatePatient : WorkGiver_MoreInjuriesTreatmentBase
{
    protected override bool CanTreat(Hediff hediff) => JobDriver_UseDefibrillator.JobCanTreat(hediff);

    protected override Job CreateJob(Pawn doctor, Pawn patient) => MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Defibrillator, JobDriver_UseDefibrillator.JobCanTreat) is Thing defibrillator
        ? JobDriver_UseDefibrillator.GetDispatcher(doctor, patient, defibrillator).CreateJob()
        : JobDriver_PerformCpr.GetDispatcher(doctor, patient).CreateJob();
}
