using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_RequireUntracked : SecondaryHediffModifier
{
    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        if (hediff.pawn.HasComp<MoreInjuryComp>())
        {
            return Disallow;
        }
        return NoChange;
    }
}
