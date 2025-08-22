using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_DisallowDeathResting : SecondaryHediffModifier
{
    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        if (ModLister.BiotechInstalled && hediff.pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest))
        {
            return 0f;
        }
        // if the pawn is not deathresting, we return the base chance
        return 1f;
    }
}