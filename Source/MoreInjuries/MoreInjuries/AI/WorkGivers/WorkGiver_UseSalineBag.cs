using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;
using MoreInjuries.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_UseSalineBag : WorkGiver_MoreInjuriesTreatmentBase
{
    private Thing? TryFindSalineBag(Pawn doctor, Pawn patient) => MedicalDeviceHelper.FindMedicalDevice
    (
        doctor, patient,
        deviceDef: JobDriver_UseSalineBag.JobDeviceDef,
        getNumberOfRequiredMedicalDevices: static patient => JobDriver_UseSalineBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false)
    );

    protected override bool IsValidPatient(Pawn doctor, Thing thing, [NotNullWhen(true)] out Pawn? patient) => base.IsValidPatient(doctor, thing, out patient)
        // base implementation already checks for NoCare, but we further require that we are allowed to use medication
        && patient.playerSettings?.medCare is not MedicalCareCategory.NoMeds;

    public override bool ShouldSkip(Pawn pawn, bool forced = false) => !KnownResearchProjectDefOf.EmergencyMedicine.IsFinished;

    public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (IsValidPatient(pawn, thing, out Pawn? patient) && TryFindSalineBag(pawn, patient) is not null)
        {
            return pawn.CanReserve(patient, ignoreOtherReservations: forced);
        }
        return false;
    }

    protected override Job CreateJob(Pawn doctor, Pawn patient) => TryFindSalineBag(doctor, patient) is Thing salineBag
        ? JobDriver_UseSalineBag.GetDispatcher(doctor, patient, salineBag, fromInventoryOnly: false, SalineTransfusionMode.Stabilize).CreateJob()
        : GetDummyDefaultJob(doctor);
}
