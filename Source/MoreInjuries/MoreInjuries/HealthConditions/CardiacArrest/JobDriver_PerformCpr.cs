using MoreInjuries.AI;
using MoreInjuries.AI.TreatmentModifiers;
using MoreInjuries.Extensions;
using MoreInjuries.KnownDefs;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.CardiacArrest;

// shared with ChokingOnBlood
public class JobDriver_PerformCpr : JobDriver_UseMedicalDevice_TargetsHediffDefs
{
    public const string JOB_LABEL_KEY = "MI_PerformCpr";

    public static HediffDef[] TargetHediffDefs { get; } = [KnownHediffDefOf.ChokingOnBlood, KnownHediffDefOf.CardiacArrest];

    protected override bool RequiresDevice => false;

    protected override HediffDef[] HediffDefs => TargetHediffDefs;

    protected override ThingDef DeviceDef => null!;

    protected override SoundDef SoundDef => KnownSoundDefOf.PerformCpr;

    protected override int BaseTendDuration => 360;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? choking = patient.health.hediffSet.hediffs.Find(static hediff => hediff.def == KnownHediffDefOf.ChokingOnBlood);
        if (choking is not null)
        {
            float severity = choking.Severity;
            float doctorSkill = doctor.GetMedicalSkillLevelOrDefault();
            // determine the factor based on the doctor's medicine skill where at level 15 the factor is 1
            float doctorSkillFactor = doctorSkill / 15f;
            doctorSkillFactor *= choking.GetTreatmentEffectivenessModifier(job.def);
            // scale severity reduction based on a sigmoid function with a random offset
            float severityReductionRaw = DiffusedSigmoid(doctorSkillFactor);
            // we only clamp after the fact to allow a theoretical increase in severity for very poorly performed CPR attempts when the negative random offset is high
            float newSeverity = Mathf.Clamp01(severity - severityReductionRaw);
            if (newSeverity > 0)
            {
                choking.Severity = newSeverity;
            }
            else
            {
                patient.health.RemoveHediff(choking);
            }
        }
        Hediff? cardiacArrest = patient.health.hediffSet.hediffs.Find(static hediff => hediff.def == KnownHediffDefOf.CardiacArrest);
        if (cardiacArrest is not null)
        {
            float severity = cardiacArrest.Severity;
            float doctorSkill = doctor.GetMedicalSkillLevelOrDefault();
            // determine the factor based on the doctor's medicine skill where at level 15 the factor is 1
            float doctorSkillFactor = doctorSkill / 15f;
            doctorSkillFactor *= cardiacArrest.GetTreatmentEffectivenessModifier(job.def);
            // scale severity reduction based on a sigmoid function with a random offset, reduced by a random factor
            float severityReductionRaw = DiffusedSigmoid(doctorSkillFactor) * Rand.Range(0.5f, 0.75f);
            // we only clamp after the fact to allow a theoretical increase in severity for very poorly performed CPR attempts when the negative random offset is high
            float newSeverity = Mathf.Clamp01(severity - severityReductionRaw);
            if (newSeverity > 0)
            {
                cardiacArrest.Severity = newSeverity;
            }
            else
            {
                patient.health.RemoveHediff(cardiacArrest);
            }
        }
    }

    private static float DiffusedSigmoid(float x) => (1f / (1f + Mathf.Exp(-10f * (x - 0.5f)))) + Rand.Range(-0.1f, 0.1f);

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient) => new JobDescriptor(doctor, patient);

    public class JobDescriptor(Pawn doctor, Pawn patient) : IJobDescriptor
    {
        public Job CreateJob()
        {
            Job job = JobMaker.MakeJob(KnownJobDefOf.PerformCpr, patient);
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