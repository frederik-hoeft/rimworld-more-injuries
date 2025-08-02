using MoreInjuries.Roslyn.Future.ThrowHelpers;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public abstract class HediffModifier_MeanTimeBetween : SecondaryHediffModifier
{
    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        if (compHandler is not IHediffComp_TickHandler compTickHandler)
        {
            Logger.ConfigError($"Handler is not a tick-based handler. Cannot evaluate MTTF chance for {hediff.LabelCap} on {hediff.pawn.Name}. Got {compHandler.GetType().Name} instead of {nameof(IHediffComp_TickHandler)}.");
            return 1f; // default chance if not a tick handler
        }
        return GetModifier(hediff, compTickHandler);
    }

    public abstract float GetModifier(Hediff hediff, IHediffComp_TickHandler compHandler);

    protected static float GetChanceFromMttf(float mttf, int tickInterval)
    {
        Throw.ArgumentOutOfRangeException.IfLessThanOrEqual(mttf, 0f);
        float lambda = 1f / mttf;
        float p = 1f - Mathf.Exp(-lambda * tickInterval);
        if (p < Mathf.Epsilon)
        {
            Logger.Warning($"{nameof(HediffModifier_MeanTimeBetween)}: MTTF is too high ({mttf} ticks). Chance is effectively 0. Returning 0 chance.");
            return 0f;
        }
        return p;
    }
}