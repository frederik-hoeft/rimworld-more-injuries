using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Fractures;

// tend all fractures on a patient or until there are no more splints
public class JobDriver_UseSplint : JobDriver_UseMedicalDevice_TargetsHediffDefs
{
    public const string JOB_LABEL = "Splint fractures";

    public static HediffDef[] TargetHediffDefs { get; } = [KnownHediffDefOf.Fracture];

    protected override bool RequiresDevice => true;

    protected override HediffDef[] HediffDefs => TargetHediffDefs;

    protected override ThingDef DeviceDef => KnownThingDefOf.Splint;

    protected override int BaseTendDuration => 450;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? fracture = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.Fracture);
        if (fracture is { Part: BodyPartRecord part })
        {
            Hediff healingFracture = HediffMaker.MakeHediff(KnownHediffDefOf.FractureHealing, patient, part);
            // base severity on doctor's medical skill and a random factor
            float medicalSkill = doctor.GetStatValue(StatDefOf.MedicalTendQuality);
            // use f(x) = 1.5 - x^2 + rand(-0.5, 0.5) to get a significantly lower severity for higher medical skill
            float severityRaw = 1.5f - (medicalSkill * medicalSkill) + Rand.Range(-0.5f, 0.5f);
            healingFracture.Severity = Mathf.Clamp(severityRaw, 0.3f, 1f);
            patient.health.AddHediff(healingFracture);
            patient.health.RemoveHediff(fracture);
            device?.DecreaseStack();
        }
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