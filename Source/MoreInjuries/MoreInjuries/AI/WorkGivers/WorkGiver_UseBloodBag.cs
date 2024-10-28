using Verse.AI;
using Verse;
using MoreInjuries.Things;
using MoreInjuries.KnownDefs;
using MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;
using MoreInjuries.HealthConditions.HeavyBleeding;
using RimWorld;

namespace MoreInjuries.AI.WorkGivers;

using static BloodLossConstants;

public class WorkGiver_UseBloodBag : WorkGiver_MoreInjuriesTreatmentBase
{
    protected override bool CanTreat(Hediff hediff) => 
        JobDriver_UseBloodBag.JobCanTreat(hediff, BLOOD_LOSS_THRESHOLD);

    private Thing? TryFindBloodBag(Pawn doctor, Pawn patient) => 
        MedicalDeviceHelper.FindMedicalDevice(doctor, patient, JobDriver_UseBloodBag.JobDeviceDef, hediff => JobDriver_UseBloodBag.JobCanTreat(hediff, BLOOD_LOSS_THRESHOLD));

    protected override bool IsValidPatient(Pawn doctor, Thing thing, out Pawn patient) => base.IsValidPatient(doctor, thing, out patient) 
        && patient.playerSettings?.medCare is not MedicalCareCategory.NoCare and not MedicalCareCategory.NoMeds;

    public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (IsValidPatient(pawn, thing, out Pawn patient) && TryFindBloodBag(pawn, patient) is not null)
        {
            foreach (Hediff hediff in patient.health.hediffSet.hediffs)
            {
                if (CanTreat(hediff))
                {
                    return pawn.CanReserve(patient, ignoreOtherReservations: forced);
                }
            }
        }
        return false;
    }

    protected override Job CreateJob(Pawn doctor, Pawn patient) => TryFindBloodBag(doctor, patient) is Thing bloodBag
        ? JobDriver_UseBloodBag.GetDispatcher(doctor, patient, bloodBag, fromInventoryOnly: false, fullyHeal: false).CreateJob()
        : GetDummyDefaultJob(doctor);
}
