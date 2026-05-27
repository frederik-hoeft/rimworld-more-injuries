using MoreInjuries.Utils;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Choking.Simulation;

internal sealed class ChokingSimulation(HediffComp_Choking context, ChokingSimulationParameters parameters)
{
    public NextChokingSimulationState MoveNext(ref readonly CurrentChokingSimulationState state)
    {
        float intervalDays = context.Properties.ChokingIntervalTicks / GenDate.TicksPerDay;

        float coughStrength = CalculateCoughStrength(state.Consciousness);

        float oldFluidBurden = Mathf.Max(0f, state.FluidBurden);

        float bleedingFluidGain = CalculateBleedingFluidGain(state.BleedRate);
        float coughTriggeredAspirationGain = CalculateCoughTriggeredAspirationGain(state.BleedRate, coughStrength);
        float coughFluidClearance = CalculateCoughFluidClearance(oldFluidBurden, state.BleedRate, coughStrength);
        float passiveFluidClearance = CalculatePassiveFluidClearance(oldFluidBurden);

        float newFluidBurden = Mathf.Max(0f, oldFluidBurden
            + bleedingFluidGain
            + coughTriggeredAspirationGain
            - coughFluidClearance
            - passiveFluidClearance);

        float chokingPressure = CalculateChokingPressure(newFluidBurden);

        float oldSeverity = Mathf.Clamp01(state.Severity);
        float severityChange = CalculateSeverityChange(oldSeverity, chokingPressure, intervalDays);

        return new NextChokingSimulationState(severityChange, newFluidBurden);
    }

    private float CalculateBleedingFluidGain(float bleedRate) =>
        parameters.FluidGainPerBleedRateOneInterval * bleedRate // normalized bleed rate contribution
        * NextMeanOneNoise(parameters.BleedingNoiseAmplitude);

    private float CalculateCoughTriggeredAspirationGain(float bleedRate, float coughStrength) =>
        parameters.FluidGainPerBleedRateOneInterval * bleedRate // normalized bleed rate contribution
        * parameters.CoughAspirationGainRatio * coughStrength
        * NextMeanOneNoise(coughStrength);

    private float CalculateCoughFluidClearance(float fluidBurden, float bleedRate, float coughStrength)
    {
        float accessibleFluidFraction = CalculateCoughAccessibleFluidFraction(fluidBurden, parameters.CoughFluidHalfEffect);

        float bleedingSuppression = CalculateBleedingSuppressionOfCough(bleedRate, parameters.CoughSuppressionBleedHalfEffect, parameters.CoughSuppressionBleedExponent);

        return parameters.FluidGainPerBleedRateOneInterval
            * parameters.CoughClearanceRatio
            * coughStrength
            * accessibleFluidFraction
            * bleedingSuppression
            * NextMeanOneNoise(coughStrength);
    }

    private float CalculateCoughStrength(float consciousness)
    {
        consciousness = Mathf.Clamp01(consciousness);
        float threshold = Mathf.Clamp01(parameters.CoughConsciousnessThreshold);
        float thresholdStrength = Mathf.Clamp(parameters.CoughStrengthAtConsciousnessThreshold, 0.001f, 0.999f);
        float sharpness = Mathf.Max(0.001f, parameters.CoughConsciousnessTransitionSharpness);

        // Derive the logistic midpoint from a more meaningful gameplay parameter:
        // "How much cough strength should remain at the unconsciousness threshold?"
        float midpoint = threshold + (Mathf.Log((1f / thresholdStrength) - 1f) / sharpness);
        float rawStrength = Mathf.Logistic(consciousness, midpoint, sharpness);

        // Normalize to exact-ish [0, 1] over RimWorld's valid consciousness range.
        // This keeps fully unconscious pawns at 0 and fully conscious pawns at 1
        // without adding a hard threshold.
        float minimumStrength = Mathf.Logistic(0f, midpoint, sharpness);
        float maximumStrength = Mathf.Logistic(1f, midpoint, sharpness);

        return Mathf.Clamp01((rawStrength - minimumStrength) / (maximumStrength - minimumStrength));
    }

    private static float CalculateCoughAccessibleFluidFraction(float fluidBurden, float fluidHalfEffect) =>
        fluidBurden / (fluidBurden + fluidHalfEffect);

    private static float CalculateBleedingSuppressionOfCough(float bleedRate, float bleedHalfEffect, float bleedExponent)
    {
        float normalizedBleedRate = bleedRate / bleedHalfEffect;
        float suppressionPressure = Mathf.Pow(normalizedBleedRate, bleedExponent);

        return 1f / (1f + suppressionPressure);
    }

    private float CalculatePassiveFluidClearance(float fluidBurden) =>
        parameters.PassiveFluidClearanceFractionPerInterval * fluidBurden;

    private float CalculateChokingPressure(float fluidBurden)
    {
        // more fluid => exponentially more pressure
        float exponent = parameters.ChokingPressureExponent;
        float numerator = Mathf.Exp(exponent * fluidBurden) - 1f;
        float denominator = Mathf.Exp(exponent) - 1f;

        return numerator / denominator;
    }

    private float CalculateSeverityChange(float currentSeverity, float chokingPressure, float intervalDays)
    {
        float progressionPerDay = parameters.ChokingSeverityProgressionPerDay * chokingPressure;
        float recoveryPerDay = parameters.ChokingSeverityRecoveryPerDay * currentSeverity / (1f + chokingPressure);

        return intervalDays * (progressionPerDay - recoveryPerDay);
    }

    private static float NextMeanOneNoise(float amplitude = 1f)
    {
        amplitude = Mathf.Clamp01(amplitude);
        return 1f + Rand.Range(-amplitude, amplitude);
    }
}
