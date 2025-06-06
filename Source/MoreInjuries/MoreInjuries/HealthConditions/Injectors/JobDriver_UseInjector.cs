using MoreInjuries.AI;
using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Injectors.Outcomes;
using MoreInjuries.KnownDefs;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Injectors;

public abstract class JobDriver_UseInjector : JobDriver_UseMedicalDevice
{
    protected override int BaseTendDuration => 60;

    protected override bool RequiresDevice => true;

    protected override SoundDef SoundDef => KnownSoundDefOf.UseAutoinjector;

    protected override bool IsTreatable(Hediff hediff) => true;

    protected override bool RequiresTreatment(Pawn patient) => true;

    // always apply exactly one injector
    protected override int GetMedicalDeviceCountToFullyHeal(Pawn patient) => 1;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        if (device?.def.GetModExtension<InjectorProps_ModExtension>() is not InjectorProps_ModExtension extension)
        {
            Logger.Warning($"{nameof(JobDriver_UseInjector)} failed to apply injector because the device is null or has no {nameof(InjectorProps_ModExtension)}");
            EndJobWith(JobCondition.Incompletable);
            return;
        }
        if (device.Destroyed)
        {
            Logger.Error($"{nameof(JobDriver_UseInjector)} failed to apply injector because the device was destroyed. What's going on?");
            EndJobWith(JobCondition.Incompletable);
            return;
        }
        device.DecreaseStack();
        if (extension.OutcomeDoers is not { Count: > 0 })
        {
            Logger.Warning($"{nameof(JobDriver_UseInjector)} has no outcome doers defined for {device.def.defName}");
            EndJobWith(JobCondition.Incompletable);
            return;
        }
        foreach (InjectionOutcomeDoer doer in extension.OutcomeDoers)
        {
            bool success = doer.TryDoOutcome(doctor, patient, device);
            if (!success)
            {
                Logger.Warning($"{nameof(JobDriver_UseInjector)} failed to apply injector with outcome doer {doer.GetType().Name}");
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
