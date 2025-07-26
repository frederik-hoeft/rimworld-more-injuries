using MoreInjuries.AI.Jobs;
using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.Hemodilution;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

public sealed class JobDriver_UseSalineBag : JobDriver_TransfusionBase
{
    public const string JOB_LABEL_KEY = "MI_UseSalineBag";

    public static ThingDef JobDeviceDef => KnownThingDefOf.SalineBag;

    protected override ThingDef DeviceDef => JobDeviceDef;

    public static int JobGetMedicalDeviceCountToFullyHeal(Pawn patient, bool fullyHeal)
    {
        int requiredTransfusionsForBloodLoss = JobGetMedicalDeviceCountToFullyHealBloodLoss(patient, JobDeviceDef, fullyHeal);
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
        float bloodLossSeverity = 0f;
        if (patient.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff bloodLoss))
        {
            bloodLossSeverity = bloodLoss.Severity;
        }
        int maxSafeTransfusions = HemodilutionEvaluator.CalculateMaximumSafeSalineTransfusions(hemodilutionSeverity, bloodLossSeverity, hemodilutionThreshold: BloodLossConstants.BLOOD_LOSS_THRESHOLD, fluidVolumePerBag);
        return Mathf.Min(requiredTransfusionsForBloodLoss, maxSafeTransfusions);
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly, bool fullyHeal) =>
        new JobDescriptor(KnownJobDefOf.UseSalineBag, doctor, patient, device, fromInventoryOnly, fullyHeal);
}