using MoreInjuries.AI.Audio;
using MoreInjuries.AI.Jobs;
using MoreInjuries.AI.TreatmentModifiers;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Choking;
using MoreInjuries.Utils;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.CardiacArrest;

// shared with ChokingOnBlood
public class JobDriver_PerformCpr : JobDriver_UseMedicalDevice_TargetsHediffDefs
{
    public const string JOB_LABEL_KEY = "MI_PerformCpr";

    private static SkillBasedOutcomeCurve CprChokingSeverityReliefCurve { get; } = new(lower: (-0.05f, 0.35f), upper: (0.15f, 0.60f));

    private static SkillBasedOutcomeCurve CprChokingFluidReductionCurve { get; } = new(lower: (0.00f, 0.06f), upper: (0.04f, 0.16f));

    public static HediffDef[] TargetHediffDefs { get; } = [KnownHediffDefOf.ChokingOnBlood, KnownHediffDefOf.CardiacArrest];

    protected override bool RequiresDevice => false;

    protected override HediffDef[] HediffDefs => TargetHediffDefs;

    protected override ThingDef DeviceDef => null!;

    protected override ISoundDefProvider<Pawn> SoundDefProvider => CachedSoundDefProvider.Of<Pawn>(KnownSoundDefOf.PerformCpr);

    protected override int BaseTendDuration => 360;

    protected override bool ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? choking = patient.health.hediffSet.hediffs.Find(static hediff => hediff.def == KnownHediffDefOf.ChokingOnBlood);
        if (choking is HediffWithComps chokingWithComps && chokingWithComps.TryGetComp(out HediffComp_Choking comp))
        {
            float oldSeverity = choking.Severity;
            float oldFluidBuildup = comp.FluidBuildup;

            // TODO: add CPR effectiveness settings here
            TreatmentInfo treatmentInfo = this.GetTreatmentInfo(doctor, choking);
            float severityReliefFraction = CprChokingSeverityReliefCurve.Evaluate(in treatmentInfo).RandomInRange;
            float severityReliefOffsetMax = 1f / 16f * Mathf.Logistic(treatmentInfo.DoctorSkill,
                CprChokingSeverityReliefCurve.LogisticMidpoint,
                CprChokingSeverityReliefCurve.LogisticSharpness);
            float severityReliefOffset = Rand.Range(0f, severityReliefOffsetMax);
            choking.Severity = Mathf.Clamp((oldSeverity * (1f - severityReliefFraction)) - severityReliefOffset, min: 0.001f, max: 1f);

            float fluidReduction = CprChokingFluidReductionCurve.Evaluate(in treatmentInfo).RandomInRange;
            float removedFluidBurden = comp.ReduceFluidBuildup(fluidReduction);

            Logger.LogDebug(
                $"CPR performed by {doctor.NameShortColored} on {patient.NameShortColored}: " +
                $"severity relief={severityReliefFraction.ToStringPercent("F1")}, " +
                $"severity={oldSeverity.ToStringPercent("F1")}->{choking.Severity.ToStringPercent("F1")}, " +
                $"fluid reduction capacity={fluidReduction.ToStringPercent("F1")}, " +
                $"reduce fluid buildup={removedFluidBurden.ToStringPercent("F1")}, " +
                $"fluid burden={oldFluidBuildup.ToStringPercent("F1")}->{comp.FluidBuildup.ToStringPercent("F1")}");
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
