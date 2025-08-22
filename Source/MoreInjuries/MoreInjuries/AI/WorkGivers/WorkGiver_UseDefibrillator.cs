using Verse.AI;
using Verse;
using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.Things;
using MoreInjuries.Defs.WellKnown;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_UseDefibrillator : WorkGiver_MoreInjuriesTreatmentBase
{
    protected override bool CanTreat(Hediff hediff) => JobDriver_UseDefibrillator.JobCanTreat(hediff);

    public override bool ShouldSkip(Pawn pawn, bool forced = false) => !KnownResearchProjectDefOf.EmergencyMedicine.IsFinished;

    protected override bool CanTreat(Pawn doctor, Pawn patient) => 
        MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Defibrillator) is not null 
        && base.CanTreat(doctor, patient);

    protected override Job CreateJob(Pawn doctor, Pawn patient) => MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Defibrillator) is Thing defibrillator
        ? JobDriver_UseDefibrillator.GetDispatcher(doctor, patient, defibrillator).CreateJob()
        : GetDummyDefaultJob(doctor);
}
