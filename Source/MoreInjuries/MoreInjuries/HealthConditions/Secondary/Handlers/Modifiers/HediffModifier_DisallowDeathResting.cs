using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_DisallowDeathResting : SecondaryHediffModifier
{
    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        if (ModLister.BiotechInstalled && hediff.pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest))
        {
            return Disallow;
        }
        return Unchanged;
    }
}
