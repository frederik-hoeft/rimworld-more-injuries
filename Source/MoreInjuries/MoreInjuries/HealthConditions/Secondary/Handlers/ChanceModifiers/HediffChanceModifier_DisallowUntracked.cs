using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

public sealed class HediffChanceModifier_DisallowUntracked : SecondaryHediffChanceModifier
{
    public override float GetModifer(Pawn pawn)
    {
        if (!pawn.HasComp<MoreInjuryComp>())
        {
            // if the pawn is not tracked by More Injuries, we return 0 chance
            return 0f;
        }
        // otherwise, we return the base chance
        return 1f;
    }
}