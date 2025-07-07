using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

// TODO: update fully-heal logic to consider healing hemodilution
public sealed class JobDriver_UseBloodBag : JobDriver_TransfusionBase
{
    public const string JOB_LABEL_KEY = "MI_UseBloodBag";

    public static ThingDef JobDeviceDef => ModLister.BiotechInstalled && MoreInjuriesMod.Settings.BiotechEnableIntegration
        ? ThingDefOf.HemogenPack
        : KnownThingDefOf.WholeBloodBag;

    protected override ThingDef DeviceDef => JobDeviceDef;

    public static int JobGetMedicalDeviceCountToFullyHeal(Pawn patient, bool fullyHeal)
    {
        int requiredTransfusionsForBloodLoss = JobGetMedicalDeviceCountToFullyHealBloodLoss(patient, JobDeviceDef, fullyHeal);
        // TODO: consider hemodilution
        return requiredTransfusionsForBloodLoss;
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly, bool fullyHeal) =>
        new JobDescriptor(KnownJobDefOf.UseBloodBag, doctor, patient, device, fromInventoryOnly, fullyHeal);
}