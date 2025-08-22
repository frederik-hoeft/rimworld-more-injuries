using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_RequireUntracked : SecondaryHediffModifier
{
    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        if (hediff.pawn.HasComp<MoreInjuryComp>())
        {
            // if the pawn is tracked by More Injuries, we return 0 chance
            return 0f;
        }
        // otherwise, we return the base chance
        return 1f;
    }
}