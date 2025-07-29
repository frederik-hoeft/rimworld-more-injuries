using MoreInjuries.AI.Audio;
using MoreInjuries.AI.Jobs;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

public abstract class JobDriver_TransfusionBase : JobDriver_OutcomeDoerBase
{
    private bool _fullyHeal;

    protected override ISoundDefProvider<Pawn> SoundDefProvider => CachedSoundDefProvider.Of<Pawn>(SoundDefOf.Recipe_Surgery);

    protected override int BaseTendDuration => 720;

    protected override int GetMedicalDeviceCountToFullyHeal(Pawn patient) => JobGetMedicalDeviceCountToFullyHealBloodLoss(patient, DeviceDef, _fullyHeal);

    protected static float GetFluidVolumePerBag(ThingDef ivFluidDef)
    {
        if (ivFluidDef.GetModExtension<TransfusionProperties_ModExtension>() is { BloodLossSeverityReduction: float bloodLossHealedPerBag })
        {
            return bloodLossHealedPerBag;
        }
        throw new InvalidOperationException($"Missing or invalid {nameof(TransfusionProperties_ModExtension)} for {ivFluidDef.defName}");
    }

    protected static int JobGetMedicalDeviceCountToFullyHealBloodLoss(Pawn patient, ThingDef ivFluidDef, bool fullyHeal)
    {
        if (patient.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff bloodLoss))
        {
            float bloodLossHealedPerBag = GetFluidVolumePerBag(ivFluidDef);
            float requiredTransfusions;
            if (fullyHeal)
            {
                requiredTransfusions = bloodLoss.Severity / bloodLossHealedPerBag;
                return Mathf.CeilToInt(requiredTransfusions);
            }
            requiredTransfusions = Mathf.Max(0f, bloodLoss.Severity - BloodLossConstants.BLOOD_LOSS_THRESHOLD) / bloodLossHealedPerBag;
            return Mathf.CeilToInt(requiredTransfusions);
        }
        return 0;
    }

    protected override bool IsTreatable(Hediff hediff)
    {
        if (_fullyHeal)
        {
            return JobCanTreat(hediff, bloodLossThreshold: 0f);
        }
        return JobCanTreat(hediff, GetFluidVolumePerBag(DeviceDef));
    }

    protected static bool JobCanTreat(Hediff hediff, float bloodLossThreshold) => hediff.def == HediffDefOf.BloodLoss && hediff.Severity > bloodLossThreshold;

    protected override bool RequiresTreatment(Pawn patient) => JobGetMedicalDeviceCountToFullyHealBloodLoss(patient, DeviceDef, _fullyHeal) > 0;

    public override void Notify_Starting()
    {
        base.Notify_Starting();

        if (Parameters is TransfusionJobParameters parameters)
        {
            _fullyHeal = parameters.fullyHeal;
        }
        else
        {
            Logger.Error($"{GetType().Name}: Missing or invalid parameters");
            EndJobWith(JobCondition.Incompletable);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref _fullyHeal, "fullyHeal");
    }

    public class JobDescriptor(JobDef jobDef, Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly, bool fullyHeal) : IJobDescriptor
    {
        public Job CreateJob()
        {
            Job job = JobMaker.MakeJob(jobDef, patient, device);
            job.count = 1;
            TransfusionJobParameters parameters = ExtendedJobParameters.Create<TransfusionJobParameters>(doctor, fromInventoryOnly);
            parameters.fullyHeal = fullyHeal;
            job.source = parameters;
            return job;
        }

        public void StartJob()
        {
            Job job = CreateJob();
            doctor.jobs.TryTakeOrderedJob(job);
        }
    }

    [SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
    protected class TransfusionJobParameters : ExtendedJobParameters
    {
        public bool fullyHeal;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref fullyHeal, "fullyHeal");
        }
    }
}
