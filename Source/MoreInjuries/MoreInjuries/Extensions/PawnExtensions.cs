using MoreInjuries.Roslyn.Future.ThrowHelpers;
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

    public static bool IsActivelyHostileTo(this Pawn pawn, Pawn other)
    {
        Throw.ArgumentNullException.IfNull(pawn);
        Throw.ArgumentNullException.IfNull(other);
        return !pawn.Downed && pawn.HostileTo(other.Faction);
    }

    // Check if pawn has a gene that grants oxygen-deficiency immunity
    // Supports: Biotech "Deathless" gene and Odyssey "Breathless" gene
    public static bool HasOxygenDeficiencyImmunity(this Pawn pawn)
    {
        // Check if pawn has genes (Biotech/Odyssey feature)
        if (!ModsConfig.BiotechActive || pawn.genes is null)
        {
            return false;
        }

        // Check for Biotech "Deathless" gene (grants immunity to suffocation and hypoxia-related conditions)
        GeneDef deathlessGene = DefDatabase<GeneDef>.GetNamedSilentFail("Deathless");
        if (deathlessGene != null && pawn.genes.HasActiveGene(deathlessGene))
        {
            return true;
        }

        // Check for Odyssey "Breathless" gene (breathes differently, immune to oxygen deficiency)
        GeneDef breathlessGene = DefDatabase<GeneDef>.GetNamedSilentFail("VacuumResistance_Total");
        if (breathlessGene != null && pawn.genes.HasActiveGene(breathlessGene))
        {
            return true;
        }

        return false;
    }
}
