using MoreInjuries.AI;
using MoreInjuries.AI.Audio;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

public class JobDriver_UseBloodBag : JobDriver_UseMedicalDevice
{
    public const string JOB_LABEL_KEY = "MI_UseBloodBag";
    private const float BLOOD_LOSS_HEALED_PER_PACK = 0.35f;

    private bool _fullyHeal;

    public static ThingDef JobDeviceDef => ModLister.BiotechInstalled && MoreInjuriesMod.Settings.BiotechEnableIntegration
        ? ThingDefOf.HemogenPack
        : KnownThingDefOf.WholeBloodBag;

    protected override bool RequiresDevice => true;

    protected override ThingDef DeviceDef => JobDeviceDef;

    protected override ISoundDefProvider<Pawn> SoundDefProvider => CachedSoundDefProvider.Of<Pawn>(SoundDefOf.Recipe_Surgery);

    protected override int BaseTendDuration => 720;

    protected override int GetMedicalDeviceCountToFullyHeal(Pawn patient) => JobGetMedicalDeviceCountToFullyHeal(patient, _fullyHeal);

    public static int JobGetMedicalDeviceCountToFullyHeal(Pawn patient, bool fullyHeal)
    {
        if (patient.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff bloodLoss))
        {
            float requiredTransfusions = bloodLoss.Severity / BLOOD_LOSS_HEALED_PER_PACK;
            if (fullyHeal)
            {
                return Mathf.CeilToInt(requiredTransfusions);
            }
            return Mathf.FloorToInt(requiredTransfusions);
        }
        return 0;
    }

    protected override bool IsTreatable(Hediff hediff)
    {
        if (_fullyHeal)
        {
            return JobCanTreat(hediff, bloodLossThreshold: 0f);
        }
        return JobCanTreat(hediff, BLOOD_LOSS_HEALED_PER_PACK);
    }

    public static bool JobCanTreat(Hediff hediff, float bloodLossThreshold) => hediff.def == HediffDefOf.BloodLoss && hediff.Severity > bloodLossThreshold;

    protected override bool RequiresTreatment(Pawn patient) => JobGetMedicalDeviceCountToFullyHeal(patient, _fullyHeal) > 0;

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

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        if (device is null || !patient.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff bloodLoss))
        {
            Logger.Warning($"{nameof(JobDriver_UseBloodBag)} failed because there was no blood bag or the patient doesn't have blood loss");
            EndJobWith(JobCondition.Incompletable);
            return;
        }
        device.DecreaseStack();
        float severity = bloodLoss.Severity - BLOOD_LOSS_HEALED_PER_PACK;
        if (severity > 0)
        {
            bloodLoss.Severity = severity;
        }
        else
        {
            patient.health.RemoveHediff(bloodLoss);
        }
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly, bool fullyHeal) =>
        new JobDescriptor(doctor, patient, device, fromInventoryOnly, fullyHeal);

    public class JobDescriptor(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly, bool fullyHeal) : IJobDescriptor
    {
        public Job CreateJob()
        {
            Job job = JobMaker.MakeJob(KnownJobDefOf.UseBloodBag, patient, device);
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

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "XML serialization naming")]
    private class TransfusionJobParameters : ExtendedJobParameters
    {
        public bool fullyHeal;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref fullyHeal, "fullyHeal");
        }
    }
}
