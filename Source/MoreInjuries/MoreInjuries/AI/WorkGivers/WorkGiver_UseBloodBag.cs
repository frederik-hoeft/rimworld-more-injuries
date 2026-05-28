using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;
using MoreInjuries.Things;
using RimWorld;
using Verse;
using Verse.AI;
using static MoreInjuries.HealthConditions.HeavyBleeding.BloodLossConstants;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_UseBloodBag : WorkGiver_MoreInjuriesTreatmentBase
{
    private Thing? TryFindBloodBag(Pawn doctor, Pawn patient) =>
        MedicalDeviceHelper.FindMedicalDevice(doctor, patient, JobDriver_UseBloodBag.JobDeviceDef, static hediff => JobDriver_UseBloodBag.JobCanTreat(hediff, BLOOD_LOSS_THRESHOLD));

    protected override bool IsValidPatient(Pawn doctor, Thing thing, [NotNullWhen(true)] out Pawn? patient) => base.IsValidPatient(doctor, thing, out patient)
        // base implementation already checks for NoCare, but we further require that we are allowed to use medication
        && patient.playerSettings?.medCare is not MedicalCareCategory.NoMeds;

    public override bool ShouldSkip(Pawn pawn, bool forced = false) => !KnownResearchProjectDefOf.BasicFirstAid.IsFinished;

    public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (IsValidPatient(pawn, thing, out Pawn? patient) && TryFindBloodBag(pawn, patient) is not null)
        {
            return pawn.CanReserve(patient, ignoreOtherReservations: forced);
        }
        return false;
    }

    protected override Job CreateJob(Pawn doctor, Pawn patient) => TryFindBloodBag(doctor, patient) is Thing bloodBag
        ? JobDriver_UseBloodBag.GetDispatcher(doctor, patient, bloodBag, fromInventoryOnly: false, fullyHeal: false).CreateJob()
        : GetDummyDefaultJob(doctor);
}
