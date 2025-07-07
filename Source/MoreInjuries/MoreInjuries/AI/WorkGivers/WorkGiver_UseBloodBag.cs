using Verse.AI;
using Verse;
using MoreInjuries.Things;
using MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;
using RimWorld;
using MoreInjuries.Defs.WellKnown;

using static MoreInjuries.HealthConditions.HeavyBleeding.BloodLossConstants;

namespace MoreInjuries.AI.WorkGivers;

// TODO: update logic to consider treating hemodilution
public class WorkGiver_UseBloodBag : WorkGiver_MoreInjuriesTreatmentBase
{
    protected override bool CanTreat(Hediff hediff) =>
        JobDriver_TransfusionBase.JobCanTreat(hediff, BLOOD_LOSS_THRESHOLD);

    private Thing? TryFindBloodBag(Pawn doctor, Pawn patient) => 
        MedicalDeviceHelper.FindMedicalDevice(doctor, patient, JobDriver_UseBloodBag.JobDeviceDef, static hediff => JobDriver_TransfusionBase.JobCanTreat(hediff, BLOOD_LOSS_THRESHOLD));

    protected override bool IsValidPatient(Pawn doctor, Thing thing, out Pawn patient) => base.IsValidPatient(doctor, thing, out patient) 
        && patient.playerSettings?.medCare is not MedicalCareCategory.NoCare and not MedicalCareCategory.NoMeds;

    public override bool ShouldSkip(Pawn pawn, bool forced = false) => !KnownResearchProjectDefOf.BasicFirstAid.IsFinished;

    public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (IsValidPatient(pawn, thing, out Pawn patient) 
            && TryFindBloodBag(pawn, patient) is not null 
            && JobDriver_UseBloodBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false) > 0)
        {
            return pawn.CanReserve(patient, ignoreOtherReservations: forced);
        }
        return false;
    }

    protected override Job CreateJob(Pawn doctor, Pawn patient) => TryFindBloodBag(doctor, patient) is Thing bloodBag
        ? JobDriver_UseBloodBag.GetDispatcher(doctor, patient, bloodBag, fromInventoryOnly: false, fullyHeal: false).CreateJob()
        : GetDummyDefaultJob(doctor);
}
