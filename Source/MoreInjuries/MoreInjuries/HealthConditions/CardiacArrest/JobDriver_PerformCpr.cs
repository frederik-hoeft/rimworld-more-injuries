using MoreInjuries.AI.Audio;
using MoreInjuries.AI.Jobs;
using MoreInjuries.AI.TreatmentModifiers;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using MoreInjuries.Utils;
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

    protected override ISoundDefProvider<Pawn> SoundDefProvider => CachedSoundDefProvider.Of<Pawn>(KnownSoundDefOf.PerformCpr);

    protected override int BaseTendDuration => 360;

    protected override bool ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? choking = patient.health.hediffSet.hediffs.Find(static hediff => hediff.def == KnownHediffDefOf.ChokingOnBlood);
        if (choking is not null)
        {
            float severity = choking.Severity;
            float doctorSkill = doctor.GetMedicalSkillLevelOrDefault();
            // determine the factor based on the doctor's medicine skill where at level 15 the factor is 1
            float doctorSkillFactor = doctorSkill / 15f;
            doctorSkillFactor *= choking.GetTreatmentEffectivenessModifier(job.def);
            // scale severity reduction based on a logistic function with a random offset
            float severityReductionRaw = DiffusedLogistic(doctorSkillFactor);
            // we only clamp after the fact to allow a theoretical increase in severity for very poorly performed CPR attempts when the negative random offset is high
            float newSeverity = Mathf.Clamp01(severity - severityReductionRaw);
            if (newSeverity > 0)
            {
                // TODO: promote fluid burden to a dedicated (hidden) hediff, and only modify that instead of the choking severity directly (addressing the cause, not the symptom)
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
            // scale severity reduction based on a logistic function with a random offset, reduced by a random factor
            float severityReductionRaw = DiffusedLogistic(doctorSkillFactor) * Rand.Range(0.5f, 0.75f);
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
        return true;
    }

    private static float DiffusedLogistic(float x) => Mathf.Logistic(x, midpoint: 0.5f, sharpness: 10f) + Rand.Range(-0.1f, 0.1f);

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
