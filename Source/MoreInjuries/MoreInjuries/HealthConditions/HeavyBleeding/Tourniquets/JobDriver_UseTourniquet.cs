using MoreInjuries.AI;
using MoreInjuries.Extensions;
using MoreInjuries.KnownDefs;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public class JobDriver_UseTourniquet : JobDriver_TourniquetBase
{
    public const string JOB_LABEL = "Apply tourniquet";

    protected override ThingDef DeviceDef => KnownThingDefOf.Tourniquet;

    protected override bool RequiresDevice => true;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device) =>
        ApplyDevice(patient, device, _bodyPart);

    internal static void ApplyDevice(Pawn patient, Thing? device, BodyPartRecord? bodyPart)
    {
        if (bodyPart is not BodyPartRecord targetPart
            || device?.def.GetModExtension<HemostasisModExtension>() is not HemostasisModExtension extension
            || patient.health.hediffSet.PartIsMissing(targetPart))
        {
            return;
        }

        List<BetterInjury> treatedInjuries = [];
        foreach (Hediff hediff in patient.health.hediffSet.hediffs)
        {
            // tourniquets can only be applied to bleeding injuries that are tendable
            if (hediff is BetterInjury { Bleeding: true } injury
                && injury.TendableNow()
                // and the injury must be on the targeted body part or one of its children
                && injury.IsOnBodyPartOrChildren(targetPart))
            {
                treatedInjuries.Add(injury);
                injury.IsBase = false;
                injury.IsCoagulationMultiplierApplied = true;
                injury.CoagulationMultiplier = extension.CoagulationMultiplier;
            }
        }
        Hediff appliedTourniquetHediff = HediffMaker.MakeHediff(KnownHediffDefOf.TourniquetApplied, patient, targetPart);
        appliedTourniquetHediff.Severity = 0.01f;
        if (appliedTourniquetHediff.TryGetComp(out TourniquetHediffComp comp))
        {
            comp.Injuries = treatedInjuries;
        }
        else
        {
            Logger.Error("Failed to get TourniquetHediffComp from applied tourniquet hediff.");
        }
        patient.health.AddHediff(appliedTourniquetHediff);
        // apply choking hediff if the tourniquet is applied to the neck
        if (targetPart.def == KnownBodyPartDefOf.Neck)
        {
            Hediff hediff = HediffMaker.MakeHediff(KnownHediffDefOf.ChokingOnTourniquet, patient);
            patient.health.AddHediff(hediff);
        }
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, BodyPartRecord bodyPart) =>
        new JobDescriptor(doctor, patient, device, bodyPart);

    public class JobDescriptor(Pawn doctor, Pawn patient, Thing device, BodyPartRecord bodyPart) : IJobDescriptor
    {
        public Job CreateJob()
        {
            Job job = JobMaker.MakeJob(KnownJobDefOf.UseTourniquet, patient, device);
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
