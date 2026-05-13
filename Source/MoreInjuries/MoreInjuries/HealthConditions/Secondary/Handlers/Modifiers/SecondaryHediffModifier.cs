using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public abstract class SecondaryHediffModifier : ISecondaryHediffModifier
{
    public abstract float GetModifier(Hediff hediff, IHediffCompHandler compHandler);
}
