using MoreInjuries.AI.TreatmentModifiers;
using MoreInjuries.Extensions;
using MoreInjuries.Utils;
using UnityEngine;
using Verse;
using OutcomeRange = (float Min, float Max);

namespace MoreInjuries.HealthConditions.Choking;

internal sealed class SkillBasedOutcomeCurve(OutcomeRange lower, OutcomeRange upper)
{
    public float LogisticMidpoint { get; init; } = 10f;

    public float LogisticSharpness { get; init; } = 0.2f;

    public float EarlySkillHalfEffect { get; init; } = 2f;

    public float EarlySkillExponent { get; init; } = 2f;

    public FloatRange MinimumOutcomeRange { get; } = new FloatRange(lower.Min, lower.Max);

    public FloatRange MaximumOutcomeRange { get; } = new FloatRange(upper.Min, upper.Max);

    public float EvaluateSkillEffect(float skill)
    {
        skill = Mathf.Max(0f, skill);

        float midSkillCompetence = Mathf.Logistic(skill, LogisticMidpoint, LogisticSharpness);
        float basicFalloff = 1f - Mathf.InverseHill(skill, EarlySkillHalfEffect, EarlySkillExponent);
        return midSkillCompetence * basicFalloff;
    }

    public FloatRange Evaluate(float skill, float effectiveness = 1f, float scale = 1f)
    {
        float skillEffect = EvaluateSkillEffect(skill) * effectiveness;
        float skillBasedMin = Mathf.Lerp(MinimumOutcomeRange.min, MinimumOutcomeRange.max, skillEffect);
        float skillBasedMax = Mathf.Lerp(MaximumOutcomeRange.min, MaximumOutcomeRange.max, skillEffect);
        return new FloatRange(skillBasedMin * scale, skillBasedMax * scale);
    }

    public FloatRange Evaluate(ref readonly TreatmentInfo treatmentInfo, float scale = 1f) =>
        Evaluate(treatmentInfo.DoctorSkill, treatmentInfo.Effectiveness, scale);

    public FloatRange Evaluate(Pawn doctor, HediffComp comp, JobDef jobDef, float scale = 1f) =>
        Evaluate(doctor.GetMedicalSkillLevelOrDefault(), comp.GetTreatmentEffectivenessModifier(jobDef), scale);

    public FloatRange  Evaluate(float skill, HediffComp comp, JobDef jobDef, float scale = 1f) =>
        Evaluate(skill, comp.GetTreatmentEffectivenessModifier(jobDef), scale);
}
