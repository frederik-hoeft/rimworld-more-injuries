using MoreInjuries.AI;
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
