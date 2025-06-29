using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

public sealed class HediffChanceModifier_DisallowDeathResting : SecondaryHediffChanceModifier
{
    public override float GetModifier(HediffComp_SecondaryCondition comp, HediffCompHandler_SecondaryCondition compHandler)
    {
        if (ModLister.BiotechInstalled && comp.parent.pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest))
        {
            return 0f;
        }
        // if the pawn is not deathresting, we return the base chance
        return 1f;
    }
}