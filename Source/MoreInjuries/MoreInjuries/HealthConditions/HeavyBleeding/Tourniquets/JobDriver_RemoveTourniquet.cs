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

    protected override bool IsTreatable(Hediff hediff) => hediff.def == KnownHediffDefOf.TourniquetApplied && hediff.Part?.woundAnchorTag == _bodyPartWoundAnchorTag;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device) =>
        ApplyDevice(patient, _bodyPartWoundAnchorTag);

    internal static void ApplyDevice(Pawn patient, BodyPartRecord? bodyPart) =>
        ApplyDevice(patient, bodyPart?.woundAnchorTag);

    private static void ApplyDevice(Pawn patient, string? bodyPartWoundAnchorTag)
    {
        if (string.IsNullOrEmpty(bodyPartWoundAnchorTag))
        {
            return;
        }
        Hediff? tourniquet = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.TourniquetApplied && hediff.Part?.woundAnchorTag == bodyPartWoundAnchorTag);
        if (tourniquet is not null)
        {
            patient.health.RemoveHediff(tourniquet);
            if (tourniquet.Part.def == KnownBodyPartDefOf.Neck)
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
            TourniquetBaseParameters parameters = ExtendedJobParameters.Create<TourniquetBaseParameters>(oneShot: true);
            parameters.woundAnchorTag = bodyPart.woundAnchorTag;
            Job job = JobMaker.MakeJob(KnownJobDefOf.RemoveTourniquet, patient);
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
