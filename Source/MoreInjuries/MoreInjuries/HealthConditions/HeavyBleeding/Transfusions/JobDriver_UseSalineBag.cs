using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

// TODO: update logic to only allow saline bags to max sustainable hemodilution
public sealed class JobDriver_UseSalineBag : JobDriver_TransfusionBase
{
    public const string JOB_LABEL_KEY = "MI_UseSalineBag";

    public static ThingDef JobDeviceDef => KnownThingDefOf.SalineBag;

    protected override ThingDef DeviceDef => JobDeviceDef;

    public static int JobGetMedicalDeviceCountToFullyHeal(Pawn patient, bool fullyHeal)
    {
        int requiredTransfusionsForBloodLoss = JobGetMedicalDeviceCountToFullyHealBloodLoss(patient, JobDeviceDef, fullyHeal);
        // TODO: consider safety margin for hemodilution
        return requiredTransfusionsForBloodLoss;
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly, bool fullyHeal) =>
        new JobDescriptor(KnownJobDefOf.UseSalineBag, doctor, patient, device, fromInventoryOnly, fullyHeal);
}