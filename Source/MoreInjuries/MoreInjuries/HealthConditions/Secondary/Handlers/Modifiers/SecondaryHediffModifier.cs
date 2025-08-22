using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public abstract class SecondaryHediffModifier
{
    public abstract float GetModifier(Hediff hediff, HediffCompHandler compHandler);
}
