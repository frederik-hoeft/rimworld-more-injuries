using MoreInjuries.AI.Jobs;
using MoreInjuries.Caching;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.Hemodilution;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

public sealed class JobDriver_UseBloodBag : JobDriver_TransfusionBase
{
    public const string JOB_LABEL_KEY = "MI_UseBloodBag"; 
    private static readonly WeakTimedDataCache<Pawn, TransfusionState, bool, TimedDataEntry<TransfusionState>> s_pawnTransfusionStateCache = new
    (
        minCacheRefreshIntervalTicks: GenTicks.TickRareInterval,
        dataProvider: static (pawn, fullyHeal) => new TransfusionState(fullyHeal, JobGetMedicalDeviceCountToFullyHeal(pawn, fullyHeal))
    );

    public static ThingDef JobDeviceDef => ModLister.BiotechInstalled && MoreInjuriesMod.Settings.BiotechEnableIntegration
        ? ThingDefOf.HemogenPack
        : KnownThingDefOf.WholeBloodBag;

    protected override ThingDef DeviceDef => JobDeviceDef;

    private protected override WeakTimedDataCache<Pawn, TransfusionState, bool, TimedDataEntry<TransfusionState>> PawnTransfusionStateCache => s_pawnTransfusionStateCache;

    public static new bool JobCanTreat(Hediff hediff, float bloodLossThreshold) => 
        JobDriver_TransfusionBase.JobCanTreat(hediff, bloodLossThreshold)
        || (hediff.def == KnownHediffDefOf.Hemodilution && hediff.Severity > BloodLossConstants.BLOOD_LOSS_THRESHOLD);

    public static int JobGetMedicalDeviceCountToFullyHeal(Pawn patient, bool fullyHeal)
    {
        int requiredTransfusionsForBloodLoss = JobGetMedicalDeviceCountToFullyHealBloodLoss(patient, JobDeviceDef, fullyHeal, out Hediff? bloodLoss);
        float bloodBagFluidVolume = GetFluidVolumePerBag(JobDeviceDef);
        float bloodLossSeverity = bloodLoss?.Severity ?? 0f;
        int requiredTransfusionsForHemodilution = 0;
        if (patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.Hemodilution, out Hediff hemodilution))
        {
            requiredTransfusionsForHemodilution = fullyHeal
                ? HemodilutionEvaluator.CalculateMaxNeededBloodTransfusionsToTreatHemodilution(hemodilution.Severity, bloodLossSeverity, bloodBagFluidVolume)
                : HemodilutionEvaluator.CalculateMinimumRequiredBloodTransfusionsToTreatHemodilution(hemodilution.Severity, bloodLossSeverity, hemodilutionThreshold: BloodLossConstants.BLOOD_LOSS_THRESHOLD, bloodBagFluidVolume);
        }
        return Mathf.Max(requiredTransfusionsForBloodLoss, requiredTransfusionsForHemodilution);
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly, bool fullyHeal) =>
        new JobDescriptor(KnownJobDefOf.UseBloodBag, doctor, patient, device, fromInventoryOnly, fullyHeal);
}