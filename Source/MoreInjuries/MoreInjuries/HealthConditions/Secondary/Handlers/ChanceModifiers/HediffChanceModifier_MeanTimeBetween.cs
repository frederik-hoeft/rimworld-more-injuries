using MoreInjuries.Roslyn.Future.ThrowHelpers;
using UnityEngine;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

public abstract class HediffChanceModifier_MeanTimeBetween : SecondaryHediffChanceModifier
{
    protected static float GetChanceFromMttf(float mttf, int tickInterval)
    {
        Throw.ArgumentOutOfRangeException.IfLessThanOrEqual(mttf, 0f);
        float lambda = 1f / mttf;
        float p = 1f - Mathf.Exp(-lambda * tickInterval);
        if (p < Mathf.Epsilon)
        {
            Logger.Warning($"{nameof(HediffChanceModifier_MeanTimeBetween)}: MTTF is too high ({mttf} ticks). Chance is effectively 0. Returning 0 chance.");
            return 0f;
        }
        return p;
    }
}