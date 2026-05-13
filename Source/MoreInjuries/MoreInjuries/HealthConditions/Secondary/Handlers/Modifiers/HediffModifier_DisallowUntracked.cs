using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_DisallowUntracked : SecondaryHediffModifier
{
    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        if (!hediff.pawn.HasComp<MoreInjuryComp>())
        {
            // if the pawn is not tracked by More Injuries, we return 0 chance
            return 0f;
        }
        // otherwise, we return the base chance
        return 1f;
    }
}
