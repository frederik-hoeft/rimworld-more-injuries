using MoreInjuries.AI.Audio;
using MoreInjuries.AI.Jobs;
using MoreInjuries.Caching;
using RimWorld;
using System.Numerics;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

public abstract class JobDriver_TransfusionBase : JobDriver_OutcomeDoerBase
{
    private static readonly DataCache<ThingDef, TransfusionProperties_ModExtension> s_transfusionPropertiesCache = new(dataProvider: static thingDef =>
    {
        if (thingDef.GetModExtension<TransfusionProperties_ModExtension>() is { } transfusionProps)
        {
            return transfusionProps;
        }
        throw new InvalidOperationException($"Missing or invalid {nameof(TransfusionProperties_ModExtension)} for {thingDef.defName}");
    });

    private bool _fullyHeal;

    protected bool FullyHeal => _fullyHeal;

    protected override ISoundDefProvider<Pawn> SoundDefProvider => CachedSoundDefProvider.Of<Pawn>(SoundDefOf.Recipe_Surgery);

    protected override int BaseTendDuration => 720;

    private protected abstract WeakTimedDataCache<Pawn, TransfusionState, bool, TimedDataEntry<TransfusionState>> PawnTransfusionStateCache { get; }

    protected static float GetFluidVolumePerBag(ThingDef ivFluidDef) => s_transfusionPropertiesCache.GetData(ivFluidDef).BloodLossSeverityReduction;

    internal protected static int JobGetMedicalDeviceCountToFullyHealBloodLoss(Pawn patient, ThingDef ivFluidDef, bool fullyHeal, out Hediff? bloodLoss)
    {
        if (patient.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out bloodLoss))
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

    protected static bool JobCanTreat(Hediff hediff, float bloodLossThreshold) => hediff.def == HediffDefOf.BloodLoss && hediff.Severity > bloodLossThreshold;

    protected override bool RequiresTreatment(Pawn patient) => GetMedicalDeviceCountToFullyHeal(patient) > 0;

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

    protected override int GetMedicalDeviceCountToFullyHeal(Pawn patient) =>
        // this method is called every tick the job is running, so use the cached version here
        JobGetMedicalDeviceCountToFullyHeal_Fast(patient, FullyHeal);

    protected int JobGetMedicalDeviceCountToFullyHeal_Fast(Pawn patient, bool fullyHeal)
    {
        WeakTimedDataCache<Pawn, TransfusionState, bool, TimedDataEntry<TransfusionState>> cache = PawnTransfusionStateCache;
        TransfusionState data = cache.GetData(patient, fullyHeal);
        if (data.FullyHeal == fullyHeal)
        {
            return data.RequiredTransfusions;
        }
        // we have a cached value, but it is not for the requested fullyHeal state
        // theoretically, there is a race condition here (TOC/TOU), but in that case, we just do a little more work than necessary
        return cache.GetData(patient, fullyHeal, forceRefresh: true).RequiredTransfusions;
    }

    protected override bool ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        bool result = base.ApplyDevice(doctor, patient, device);
        // ensure the pawn doesn't clutter the cache for an undetermined amount of time
        // also ensures that the cache is refreshed if the job is repeated
        PawnTransfusionStateCache.RemoveData(patient);
        return result;
    }

    protected class JobDescriptor(JobDef jobDef, Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly, bool fullyHeal) : IJobDescriptor
    {
        public Job CreateJob()
        {
            Job job = JobMaker.MakeJob(jobDef, patient, device);
            job.count = 1;
            TransfusionJobParameters parameters = CreateParameters(doctor, fromInventoryOnly);
            parameters.fullyHeal = fullyHeal;
            job.source = parameters;
            return job;
        }

        protected virtual TransfusionJobParameters CreateParameters(Pawn doctor, bool fromInventoryOnly) =>
            ExtendedJobParameters.Create<TransfusionJobParameters>(doctor, fromInventoryOnly);

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

    private protected readonly record struct TransfusionState(bool FullyHeal, int RequiredTransfusions);
}
