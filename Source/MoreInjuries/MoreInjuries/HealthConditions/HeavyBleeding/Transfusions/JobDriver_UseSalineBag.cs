using MoreInjuries.AI.Jobs;
using MoreInjuries.Caching;
using MoreInjuries.Debug;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.Hemodilution;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

public sealed class JobDriver_UseSalineBag : JobDriver_TransfusionBase
{
    public const string JOB_LABEL_KEY = "MI_UseSalineBag";
    private static readonly WeakTimedDataCache<Pawn, TransfusionState, bool, TimedDataEntry<TransfusionState>> s_pawnTransfusionStateCache = new
    (
        minCacheRefreshIntervalTicks: GenTicks.TickRareInterval,
        dataProvider: static (pawn, fullyHeal) => new TransfusionState(fullyHeal, JobGetMedicalDeviceCountToFullyHeal(pawn, fullyHeal))
    );

    public static ThingDef JobDeviceDef => KnownThingDefOf.SalineBag;

    protected override ThingDef DeviceDef => JobDeviceDef;

    protected override int BaseTendDuration => 320;

    private protected override WeakTimedDataCache<Pawn, TransfusionState, bool, TimedDataEntry<TransfusionState>> PawnTransfusionStateCache => s_pawnTransfusionStateCache;

    protected override int GetMedicalDeviceCountToFullyHeal(Pawn patient)
    {
        // if we are forcing a single transfusion, we return 1 here.
        if (_oneShot && !_oneShotUsed)
        {
            // If we are forced to use a single transfusion, we return 1 here.
            // This is to ensure that the job will always be created and executed exactly once,
            // even if the patient is already fully healed.
            return 1;
        }
        // otherwise, run the usual safety checks and calculations
        return base.GetMedicalDeviceCountToFullyHeal(patient);
    }

    public static int JobGetMedicalDeviceCountToFullyHeal(Pawn patient, bool fullyHeal)
    {
        int requiredTransfusionsForBloodLoss = JobGetMedicalDeviceCountToFullyHealBloodLoss(patient, JobDeviceDef, fullyHeal, out Hediff? bloodLoss);
        if (requiredTransfusionsForBloodLoss <= 0)
        {
            return 0;
        }
        float fluidVolumePerBag = GetFluidVolumePerBag(JobDeviceDef);
        float hemodilutionSeverity = 0f;
        if (patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.Hemodilution, out Hediff hemodilution))
        {
            hemodilutionSeverity = hemodilution.Severity;
        }
        // will never be null here, otherwise we would have returned 0 above
        DebugAssert.IsNotNull(bloodLoss);
        float bloodLossSeverity = bloodLoss.Severity;
        int maxSafeTransfusions = HemodilutionEvaluator.CalculateMaximumSafeSalineTransfusions(
            hemodilutionSeverity, 
            bloodLossSeverity, 
            hemodilutionThreshold: BloodLossConstants.BLOOD_LOSS_THRESHOLD, 
            fluidVolumePerBag);
        int requiredTransfusions = Mathf.Min(requiredTransfusionsForBloodLoss, maxSafeTransfusions);
        return requiredTransfusions;
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly, SalineTransfusionMode transfusionMode) =>
        new SalineTransfusionJobDescriptor(doctor, patient, device, fromInventoryOnly, transfusionMode);

    private sealed class SalineTransfusionJobDescriptor(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly, SalineTransfusionMode transfusionMode) 
        : JobDescriptor(KnownJobDefOf.UseSalineBag, doctor, patient, device, fromInventoryOnly, fullyHeal: transfusionMode is SalineTransfusionMode.MaximumSafeDose)
    {
        protected override TransfusionJobParameters CreateParameters(Pawn doctor, bool fromInventoryOnly) =>
            ExtendedJobParameters.Create<TransfusionJobParameters>(doctor, fromInventoryOnly, oneShot: transfusionMode is SalineTransfusionMode.ForceTransfusion);
    }
}
