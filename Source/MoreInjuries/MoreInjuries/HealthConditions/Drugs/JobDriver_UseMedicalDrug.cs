using MoreInjuries.AI;
using MoreInjuries.AI.Audio;
using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Drugs.Outcomes;
using MoreInjuries.KnownDefs;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Drugs;

public abstract class JobDriver_UseMedicalDrug : JobDriver_UseMedicalDevice
{
    protected override int BaseTendDuration => 40;

    protected override bool RequiresDevice => true;

    protected override ISoundDefProvider<Pawn> SoundDefProvider => CachedSoundDefProvider.Of<Pawn>(KnownSoundDefOf.UseAutoinjector);

    protected override bool IsTreatable(Hediff hediff) => true;

    protected override bool RequiresTreatment(Pawn patient) => true;

    // always apply exactly one injector
    protected override int GetMedicalDeviceCountToFullyHeal(Pawn patient) => 1;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        if (device?.def.GetModExtension<MedicalDrugProps_ModExtension>() is not MedicalDrugProps_ModExtension extension)
        {
            Logger.Warning($"{nameof(JobDriver_UseMedicalDrug)} failed to apply drug because the device is null or has no {nameof(MedicalDrugProps_ModExtension)}");
            EndJobWith(JobCondition.Incompletable);
            return;
        }
        if (device.Destroyed)
        {
            Logger.Error($"{nameof(JobDriver_UseMedicalDrug)} failed to apply drug because the device was destroyed. What's going on?");
            EndJobWith(JobCondition.Incompletable);
            return;
        }
        device.DecreaseStack();
        if (extension.OutcomeDoers is not { Count: > 0 })
        {
            Logger.Warning($"{nameof(JobDriver_UseMedicalDrug)} has no outcome doers defined for {device.def.defName}");
            EndJobWith(JobCondition.Incompletable);
            return;
        }
        foreach (DrugOutcomeDoer doer in extension.OutcomeDoers)
        {
            bool success = doer.TryDoOutcome(doctor, patient, device);
            if (!success)
            {
                Logger.Warning($"{nameof(JobDriver_UseMedicalDrug)} failed to apply injector with outcome doer {doer.GetType().Name}");
                EndJobWith(JobCondition.Incompletable);
                return;
            }
        }
    }

    protected class JobDescriptor(JobDef jobDef, Pawn doctor, Pawn patient, Thing? device) : IJobDescriptor
    {
        public Job CreateJob()
        {
            ExtendedJobParameters parameters = ExtendedJobParameters.Create<ExtendedJobParameters>(doctor, oneShot: true);
            Job job = JobMaker.MakeJob(jobDef, patient, device);
            job.count = 1;
            job.source = parameters;
            return job;
        }

        public void StartJob()
        {
            Job job = CreateJob();
            doctor.jobs.TryTakeOrderedJob(job);
        }
    }
}
