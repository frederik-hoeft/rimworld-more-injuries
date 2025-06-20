using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

public abstract class SecondaryHediffChanceModifier
{
    public abstract float GetModifer(Pawn pawn);
}
