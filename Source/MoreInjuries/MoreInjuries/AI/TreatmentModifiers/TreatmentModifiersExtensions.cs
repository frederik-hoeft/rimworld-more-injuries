using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

public static class TreatmentModifiersExtensions
{
    public static float GetTreatmentEffectivenessModifier(this Hediff hediff, JobDef jobDef)
    {
        float effectiveness = 1f;
        if (hediff.def.GetModExtension<TreatmentModifiers_ModExtension>() is { } modExtension)
        {
            effectiveness = modExtension.GetTreatmentEffectiveness(jobDef, hediff);
        }
        return effectiveness;
    }
}