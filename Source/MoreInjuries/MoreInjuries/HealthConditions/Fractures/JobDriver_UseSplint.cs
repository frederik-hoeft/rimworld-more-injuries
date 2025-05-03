using MoreInjuries.AI;
using MoreInjuries.Extensions;
using MoreInjuries.KnownDefs;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Fractures;

// tend all fractures on a patient or until there are no more splints
public class JobDriver_UseSplint : JobDriver_UseMedicalDevice_TargetsHediffDefs
{
    public const string JOB_LABEL_KEY = "MI_UseSplint";

    public static HediffDef[] TargetHediffDefs { get; } = [KnownHediffDefOf.Fracture];

    protected override bool RequiresDevice => true;

    protected override HediffDef[] HediffDefs => TargetHediffDefs;

    protected override ThingDef DeviceDef => KnownThingDefOf.Splint;

    protected override int BaseTendDuration => 450;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? fracture = patient.health.hediffSet.hediffs.Find(static hediff => hediff.def == KnownHediffDefOf.Fracture);
        if (fracture is { Part: BodyPartRecord part })
        {
            SplintFracture(doctor, patient, fracture, part);
            device?.DecreaseStack();
        }
    }

    internal static void SplintFracture(Pawn doctor, Pawn patient, Hediff fracture, BodyPartRecord part, float severityOffset = 0f)
    {
        Hediff healingFracture = HediffMaker.MakeHediff(KnownHediffDefOf.FractureHealing, patient, part);
        // base severity on doctor's medical skill and a random factor
        float medicalSkill = doctor.GetStatValue(StatDefOf.MedicalTendQuality);
        // use f(x) = 1 - 0.125 x^2 + rand(-0.1, 0.2) to get a slightly lower severity for higher medical skill
        float severityRaw = 1f - (0.125f * medicalSkill * medicalSkill);
        healingFracture.Severity = Mathf.Clamp(severityRaw, 0.5f, 1f) + Rand.Range(-0.1f, 0.2f) + severityOffset;
        patient.health.AddHediff(healingFracture);
        patient.health.RemoveHediff(fracture);
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) => new JobDescriptor(doctor, patient, device);

    public class JobDescriptor(Pawn doctor, Pawn patient, Thing device) : IJobDescriptor
    {
        public Job CreateJob()
        {
            Job job = JobMaker.MakeJob(KnownJobDefOf.UseSplint, patient, device);
            job.count = 1;
            return job;
        }

        public void StartJob()
        {
            Job job = CreateJob();
            doctor.jobs.TryTakeOrderedJob(job);
        }
    }
}