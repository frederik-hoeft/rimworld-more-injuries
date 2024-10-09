using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

public class JobDriver_PerformCpr : JobDriver_UseMedicalDevice
{
    public static HediffDef[] TargetHediffDefs { get; } = [KnownHediffDefOf.ChokingOnBlood, KnownHediffDefOf.CardiacArrest];

    protected override bool RequiresDevice => false;

    protected override HediffDef[] HediffDefs => TargetHediffDefs;

    protected override ThingDef DeviceDef => null!;

    protected override int BaseTendDuration => 240;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? choking = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.ChokingOnBlood);
        if (choking is not null)
        {
            float severity = choking.Severity;
            float doctorSkill = doctor.skills.GetSkill(SkillDefOf.Medicine).Level;
            // determine the factor based on the doctor's medicine skill where at level 15 the factor is 1
            float doctorSkillFactor = doctorSkill / 15f;
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

        Hediff? cardiacArrest = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.CardiacArrest);
        if (cardiacArrest is not null)
        {
            float severity = cardiacArrest.Severity;
            float doctorSkill = doctor.skills.GetSkill(SkillDefOf.Medicine).Level;
            // determine the factor based on the doctor's medicine skill where at level 15 the factor is 1
            float doctorSkillFactor = doctorSkill / 15f;
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

    private static float DiffusedSigmoid(float x) => 1f / (1f + Mathf.Exp(-10f * (x - 0.5f))) + Rand.Range(-0.1f, 0.1f);
}