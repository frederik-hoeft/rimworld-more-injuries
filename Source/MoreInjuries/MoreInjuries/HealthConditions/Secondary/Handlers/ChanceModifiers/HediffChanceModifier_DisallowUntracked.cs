using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

public sealed class HediffChanceModifier_DisallowUntracked : SecondaryHediffChanceModifier
{
    public override float GetModifier(HediffComp_SecondaryCondition comp, HediffCompHandler_SecondaryCondition compHandler)
    {
        if (!comp.parent.pawn.HasComp<MoreInjuryComp>())
        {
            // if the pawn is not tracked by More Injuries, we return 0 chance
            return 0f;
        }
        // otherwise, we return the base chance
        return 1f;
    }
}