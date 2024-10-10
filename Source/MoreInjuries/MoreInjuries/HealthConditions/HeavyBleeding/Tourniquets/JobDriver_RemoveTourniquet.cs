using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public class JobDriver_RemoveTourniquet : JobDriver_TourniquetBase
{
    public const string JOB_LABEL = "Remove tourniquet";

    protected override ThingDef DeviceDef => null!;

    protected override bool RequiresDevice => false;

    protected override bool IsTreatable(Hediff hediff) => hediff.def == KnownHediffDefOf.TourniquetApplied && hediff.Part == _bodyPart;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device) =>
        ApplyDevice(patient, _bodyPart);

    internal static void ApplyDevice(Pawn patient, BodyPartRecord? bodyPart)
    {
        Hediff? tourniquet = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.TourniquetApplied && hediff.Part == bodyPart);
        if (tourniquet is not null)
        {
            patient.health.RemoveHediff(tourniquet);
            if (bodyPart?.def == KnownBodyPartDefOf.Neck)
            {
                Hediff? choking = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.ChokingOnTourniquet);
                if (choking is not null)
                {
                    patient.health.RemoveHediff(choking);
                }
            }
        }
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, BodyPartRecord bodyPart) =>
        new JobDescriptor(doctor, patient, bodyPart);

    public class JobDescriptor(Pawn doctor, Pawn patient, BodyPartRecord bodyPart) : IJobDescriptor
    {
        public Job CreateJob()
        {
            Job job = JobMaker.MakeJob(KnownJobDefOf.RemoveTourniquet, patient);
            job.count = 1;
            s_transientJobParameters.Add(job, new ExtendedJobParameters(OneShot: true));
            s_transientTargetParts.Add(job, bodyPart);
            return job;
        }

        public void StartJob()
        {
            Job job = CreateJob();
            doctor.jobs.TryTakeOrderedJob(job);
        }
    }
}
