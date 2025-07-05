using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.BrainDamage;

public sealed class HediffComp_PersonalityShift : HediffComp
{
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        // re-shuffle the pawn's skills based on the severity of the hediff
        if (parent.pawn.skills?.skills is null)
        {
            return;
        }
        float severity = parent.Severity;
        foreach (SkillRecord skill in parent.pawn.skills.skills)
        {
            // apply a random shift to the skill level based on the severity of the hediff
            float shift = Rand.Range(-severity, severity) * 10;
            skill.Level += (int)shift;
        }
    }
}