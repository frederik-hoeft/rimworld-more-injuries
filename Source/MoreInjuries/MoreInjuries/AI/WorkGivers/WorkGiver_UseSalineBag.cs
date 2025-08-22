using Verse.AI;
using Verse;
using MoreInjuries.Things;
using MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;
using RimWorld;
using MoreInjuries.Defs.WellKnown;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_UseSalineBag : WorkGiver_MoreInjuriesTreatmentBase
{
    private Thing? TryFindSalineBag(Pawn doctor, Pawn patient) => MedicalDeviceHelper.FindMedicalDevice(
        doctor, 
        patient, 
        JobDriver_UseSalineBag.JobDeviceDef, 
        static patient => JobDriver_UseSalineBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false));

    protected override bool IsValidPatient(Pawn doctor, Thing thing, out Pawn patient) => base.IsValidPatient(doctor, thing, out patient) 
        && patient.playerSettings?.medCare is not MedicalCareCategory.NoCare and not MedicalCareCategory.NoMeds;

    public override bool ShouldSkip(Pawn pawn, bool forced = false) => !KnownResearchProjectDefOf.EmergencyMedicine.IsFinished;

    public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (IsValidPatient(pawn, thing, out Pawn patient) && TryFindSalineBag(pawn, patient) is not null)
        {
            return pawn.CanReserve(patient, ignoreOtherReservations: forced);
        }
        return false;
    }

    protected override Job CreateJob(Pawn doctor, Pawn patient) => TryFindSalineBag(doctor, patient) is Thing salineBag
        ? JobDriver_UseSalineBag.GetDispatcher(doctor, patient, salineBag, fromInventoryOnly: false, SalineTransfusionMode.Stabilize).CreateJob()
        : GetDummyDefaultJob(doctor);
}
