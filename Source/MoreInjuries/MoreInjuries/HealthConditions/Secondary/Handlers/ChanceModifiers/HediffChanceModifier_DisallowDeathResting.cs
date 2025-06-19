using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

public sealed class HediffChanceModifier_DisallowDeathResting : SecondaryHediffChanceModifier
{
    public override float GetModifer(Pawn pawn)
    {
        if (ModLister.BiotechInstalled && pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest))
        {
            return 0f;
        }
        // if the pawn is not deathresting, we return the base chance
        return 1f;
    }
}