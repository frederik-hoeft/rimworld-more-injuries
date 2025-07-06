using MoreInjuries.AI;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Secondary;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public class JobDriver_UseTourniquet : JobDriver_TourniquetBase
{
    public const string JOB_LABEL_KEY = "MI_UseTourniquet";

    protected override ThingDef DeviceDef => KnownThingDefOf.Tourniquet;

    protected override bool RequiresDevice => true;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device) =>
        ApplyDevice(patient, device, _bodyPartKey);

    // we don't target a specific hediff
    protected override bool IsTreatable(Hediff hediff) => true;

    // the patient requires treatment until a tourniquet is applied to the targeted body part
    protected override bool RequiresTreatment(Pawn patient) => 
        !patient.health.hediffSet.hediffs.Any(hediff => hediff.def == KnownHediffDefOf.TourniquetApplied && GetUniqueBodyPartKey(hediff.Part) == _bodyPartKey);

    internal static void ApplyDevice(Pawn patient, Thing? device, BodyPartRecord? bodyPart) =>
        ApplyDevice(patient, device, GetUniqueBodyPartKey(bodyPart));

    private static void ApplyDevice(Pawn patient, Thing? device, string? bodyPartKey)
    {
        if (string.IsNullOrEmpty(bodyPartKey)
            || device?.def.GetModExtension<HemostasisModExtension>() is not HemostasisModExtension extension
            || patient.RaceProps.body.AllParts.Find(part => GetUniqueBodyPartKey(part) == bodyPartKey) is not BodyPartRecord targetPart)
        {
            Logger.Warning($"Failed to apply tourniquet because of invalid parameters: {patient}, {device}, {bodyPartKey}");
            return;
        }
        Hediff appliedTourniquetHediff = HediffMaker.MakeHediff(KnownHediffDefOf.TourniquetApplied, patient, targetPart);
        appliedTourniquetHediff.Severity = 0.01f;
        if (appliedTourniquetHediff.TryGetComp(out TourniquetHediffComp comp))
        {
            comp.CoagulationMultiplier = extension.CoagulationMultiplier;
        }
        else
        {
            Logger.Error("Failed to get TourniquetHediffComp from applied tourniquet hediff.");
            return;
        }
        patient.health.AddHediff(appliedTourniquetHediff);
        comp.ReapplyEffectsToWounds();
        // apply choking hediff if the tourniquet is applied to the neck
        if (targetPart.def == KnownBodyPartDefOf.Neck)
        {
            Hediff chokingHediff = HediffMaker.MakeHediff(KnownHediffDefOf.ChokingOnTourniquet, patient);
            if (chokingHediff.TryGetComp(out HediffComp_CausedBy? causedBy))
            {
                causedBy!.AddCause(appliedTourniquetHediff);
            }
            patient.health.AddHediff(chokingHediff);
        }
        device.DecreaseStack();
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, BodyPartRecord bodyPart) =>
        new JobDescriptor(doctor, patient, device, bodyPart);

    public class JobDescriptor(Pawn doctor, Pawn patient, Thing device, BodyPartRecord bodyPart) : IJobDescriptor
    {
        public Job CreateJob()
        {
            TourniquetBaseParameters parameters = ExtendedJobParameters.Create<TourniquetBaseParameters>(doctor, oneShot: true);
            // the only thing that is persistent and unique between the limbs is the anchor tag
            parameters.bodyPartKey = GetUniqueBodyPartKey(bodyPart);
            Job job = JobMaker.MakeJob(KnownJobDefOf.UseTourniquet, patient, device);
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
