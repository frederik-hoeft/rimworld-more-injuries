using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

public interface ISecondaryHediffModifier
{
    float GetModifier(Hediff hediff, HediffCompHandler compHandler);
}
