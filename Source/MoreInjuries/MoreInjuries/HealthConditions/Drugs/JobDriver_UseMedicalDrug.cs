using MoreInjuries.AI.Audio;
using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Drugs;

public abstract class JobDriver_UseMedicalDrug : JobDriver_OutcomeDoerBase
{
    protected override int BaseTendDuration => 40;

    protected override ISoundDefProvider<Pawn> SoundDefProvider => CachedSoundDefProvider.Of<Pawn>(KnownSoundDefOf.UseAutoinjector);

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
