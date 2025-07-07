using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public class JobDriver_RemoveTourniquet : JobDriver_TourniquetBase
{
    public const string JOB_LABEL_KEY = "MI_RemoveTourniquet";

    protected override ThingDef DeviceDef => null!;

    protected override bool RequiresDevice => false;

    protected override bool IsTreatable(Hediff hediff) => hediff.def == KnownHediffDefOf.TourniquetApplied && GetUniqueBodyPartKey(hediff.Part) == _bodyPartKey;

    protected override bool ApplyDevice(Pawn doctor, Pawn patient, Thing? device) =>
        ApplyDevice(patient, _bodyPartKey);

    internal static void ApplyDevice(Pawn patient, BodyPartRecord? bodyPart) =>
        ApplyDevice(patient, GetUniqueBodyPartKey(bodyPart));

    private static bool ApplyDevice(Pawn patient, string? bodyPartKey)
    {
        if (string.IsNullOrEmpty(bodyPartKey))
        {
            return false;
        }
        Hediff? tourniquet = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.TourniquetApplied && GetUniqueBodyPartKey(hediff.Part) == bodyPartKey);
        if (tourniquet is not null)
        {
            patient.health.RemoveHediff(tourniquet);
            if (tourniquet.Part.def == KnownBodyPartDefOf.Neck)
            {
                Hediff? choking = patient.health.hediffSet.hediffs.Find(static hediff => hediff.def == KnownHediffDefOf.ChokingOnTourniquet);
                if (choking is not null)
                {
                    patient.health.RemoveHediff(choking);
                }
            }
            // spawn a tourniquet item on the ground
            Thing tourniquetThing = ThingMaker.MakeThing(KnownThingDefOf.Tourniquet);
            GenPlace.TryPlaceThing(tourniquetThing, patient.Position, patient.Map, ThingPlaceMode.Near);
            return true;
        }
        return false;
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, BodyPartRecord bodyPart) =>
        new JobDescriptor(doctor, patient, bodyPart);

    public class JobDescriptor(Pawn doctor, Pawn patient, BodyPartRecord bodyPart) : IJobDescriptor
    {
        public Job CreateJob()
        {
            TourniquetBaseParameters parameters = ExtendedJobParameters.Create<TourniquetBaseParameters>(doctor, oneShot: true);
            parameters.bodyPartKey = GetUniqueBodyPartKey(bodyPart);
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
