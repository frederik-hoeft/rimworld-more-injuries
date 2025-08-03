using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public abstract class JobDriver_RemoveTourniquetBase : JobDriver_TourniquetBase
{
    private float _ischemiaSeverity;

    protected float IschemiaSeverity => _ischemiaSeverity;

    protected override ThingDef DeviceDef => null!;

    protected override bool RequiresDevice => false;

    internal static void ApplyDevice(Pawn patient, BodyPartRecord? bodyPart) => ApplyDevice(patient, GetUniqueBodyPartKey(bodyPart));

    protected static Hediff? GetTourniquetHediff(Pawn patient, string bodyPartKey) => patient.health.hediffSet.hediffs.Find(hediff => 
        hediff.def == KnownHediffDefOf.TourniquetApplied 
        && GetUniqueBodyPartKey(hediff.Part) == bodyPartKey);

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref _ischemiaSeverity, "ischemiaSeverity");
    }

    private static bool ApplyDevice(Pawn patient, string? bodyPartKey)
    {
        if (string.IsNullOrEmpty(bodyPartKey) || GetTourniquetHediff(patient, bodyPartKey!) is not { } tourniquet)
        {
            return false;
        }
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

    protected override bool ApplyDevice(Pawn doctor, Pawn patient, Thing? device) => ApplyDevice(patient, _bodyPartKey);

    protected override bool IsTreatable(Hediff hediff) => hediff.def == KnownHediffDefOf.TourniquetApplied && GetUniqueBodyPartKey(hediff.Part) == _bodyPartKey;

    public override void Notify_Starting()
    {
        base.Notify_Starting();

        if (string.IsNullOrEmpty(_bodyPartKey) || GetTourniquetHediff(Patient, _bodyPartKey!) is not { } tourniquet)
        {
            _ischemiaSeverity = 0f;
        }
        else
        {
            _ischemiaSeverity = tourniquet.Severity;
        }
    }

    protected class JobDescriptor(JobDef jobDef, Pawn doctor, Pawn patient, BodyPartRecord bodyPart) : IJobDescriptor
    {
        public Job CreateJob()
        {
            TourniquetBaseParameters parameters = ExtendedJobParameters.Create<TourniquetBaseParameters>(doctor, oneShot: true);
            parameters.bodyPartKey = GetUniqueBodyPartKey(bodyPart);
            Job job = JobMaker.MakeJob(jobDef, patient);
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
