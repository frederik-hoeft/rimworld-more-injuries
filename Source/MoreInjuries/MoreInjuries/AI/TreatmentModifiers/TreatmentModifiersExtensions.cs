using MoreInjuries.Extensions;
using Verse;
using Verse.AI;

namespace MoreInjuries.AI.TreatmentModifiers;

public static class TreatmentModifiersExtensions
{
    public static TreatmentInfo GetTreatmentInfo(this JobDriver jobDriver, Pawn doctor, Hediff hediff)
    {
        float doctorSkill = doctor.GetMedicalSkillLevelOrDefault();
        float effectivenessModifier = hediff.GetTreatmentEffectivenessModifier(jobDriver.job.def);
        return new TreatmentInfo(doctorSkill, effectivenessModifier);
    }

    public static TreatmentInfo GetTreatmentInfo(this JobDriver jobDriver, Pawn doctor, HediffComp comp) =>
        jobDriver.GetTreatmentInfo(doctor, comp.parent);

    public static float GetTreatmentEffectivenessModifier(this Hediff hediff, JobDef jobDef)
    {
        float effectiveness = 1f;
        if (hediff.def.GetModExtension<TreatmentModifiers_ModExtension>() is { } modExtension)
        {
            effectiveness = modExtension.GetTreatmentEffectiveness(jobDef, hediff);
        }
        return effectiveness;
    }

    public static float GetTreatmentEffectivenessModifier(this HediffComp comp, JobDef jobDef) =>
        comp.parent.GetTreatmentEffectivenessModifier(jobDef);
}
