using RimWorld;
using Verse;

namespace MoreInjuries.Extensions;

public static class PawnExtensions
{
    public static int GetMedicalSkillLevelOrDefault(this Pawn pawn, int defaultValue = 10)
    {
        if (pawn.skills?.GetSkill(SkillDefOf.Medicine) is SkillRecord skill)
        {
            return skill.Level;
        }
        if (!pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
        {
            // e.g., paramedic mechanoids / non-human pawns that can be doctors
            // assume they're good at it
            return 15;
        }
        return defaultValue;
    }
}
