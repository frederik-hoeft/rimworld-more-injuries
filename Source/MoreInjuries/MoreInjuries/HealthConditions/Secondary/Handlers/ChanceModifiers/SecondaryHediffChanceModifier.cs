using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

public abstract class SecondaryHediffChanceModifier
{
    public abstract float GetModifier(HediffComp_SecondaryCondition comp, HediffCompHandler_SecondaryCondition compHandler);
}
