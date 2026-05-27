using MoreInjuries.Roslyn.Future.ThrowHelpers;
using MoreInjuries.Utils;
using RimWorld;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Choking.Simulation;

internal sealed class ChokingSimulation
{
    private const float FLUID_MODEL_REFERENCE_RESOLUTION = 750f;
    private readonly ChokingSimulationParameters _parameters;

    public ChokingSimulation(HediffComp_Choking context, ChokingSimulationParameters parameters)
    {
        Throw.ArgumentOutOfRangeException.IfNegativeOrZero(context.Properties.TickInterval);
        TickInterval = context.Properties.TickInterval;
        _parameters = parameters;
        ResolutionScale = TickInterval / FLUID_MODEL_REFERENCE_RESOLUTION;
        IntervalDays = TickInterval / GenDate.TicksPerDay;
        FluidGainPerBleedRateSimulationStep = parameters.FluidGainPerBleedRateRareTick * ResolutionScale;
    }

    public float IntervalDays { get; }

    public float TickInterval { get; }

    private float ResolutionScale { get; }

    private float FluidGainPerBleedRateSimulationStep { get; }

    public NextChokingSimulationState MoveNext(ref readonly CurrentChokingSimulationState state)
    {
        float coughStrength = CalculateCoughStrength(state.Consciousness);

        float oldFluidBurden = Mathf.Max(0f, state.FluidBurden);

        float bleedingFluidGain = CalculateBleedingFluidGain(state.BleedRate);
        float coughTriggeredAspirationGain = CalculateCoughTriggeredAspirationGain(state.BleedRate, coughStrength);
        float coughFluidClearance = CalculateCoughFluidClearance(oldFluidBurden, state.BleedRate, coughStrength);
        float passiveFluidClearance = CalculatePassiveFluidClearance(oldFluidBurden);

        float newFluidBurden = Mathf.Clamp
        (
            value: oldFluidBurden
                + bleedingFluidGain
                + coughTriggeredAspirationGain
                - coughFluidClearance
                - passiveFluidClearance,
            min: 0f,
            max: _parameters.MaximumFluidBurden
        );

        float chokingPressure = CalculateChokingPressure(newFluidBurden);

        float oldSeverity = Mathf.Clamp01(state.Severity);
        float severityChange = CalculateSeverityChange(oldSeverity, chokingPressure, newFluidBurden, state.BleedRate, coughStrength);

        return new NextChokingSimulationState(severityChange, newFluidBurden);
    }

    private float CalculateBleedingFluidGain(float bleedRate) =>
        FluidGainPerBleedRateSimulationStep * bleedRate // normalized bleed rate contribution
        * NextMeanOneNoise(_parameters.BleedingNoiseAmplitude);

    private float CalculateCoughTriggeredAspirationGain(float bleedRate, float coughStrength) =>
        FluidGainPerBleedRateSimulationStep * bleedRate // normalized bleed rate contribution
        * _parameters.CoughAspirationGainRatio * coughStrength
        * NextMeanOneNoise(coughStrength);

    private float CalculateCoughFluidClearance(float fluidBurden, float bleedRate, float coughStrength)
    {
        float accessibleFluidFraction = CalculateCoughAccessibleFluidFraction(fluidBurden, _parameters.CoughFluidHalfEffect);

        float bleedingSuppression = CalculateBleedingSuppressionOfCough(bleedRate, _parameters.CoughSuppressionBleedHalfEffect, _parameters.CoughSuppressionBleedExponent);

        return FluidGainPerBleedRateSimulationStep
            * _parameters.CoughClearanceRatio
            * coughStrength
            * accessibleFluidFraction
            * bleedingSuppression
            * NextMeanOneNoise(coughStrength);
    }

    private float CalculateCoughStrength(float consciousness)
    {
        consciousness = Mathf.Clamp01(consciousness);
        float threshold = Mathf.Clamp01(_parameters.CoughConsciousnessThreshold);
        float thresholdStrength = Mathf.Clamp(_parameters.CoughStrengthAtConsciousnessThreshold, 0.001f, 0.999f);
        float sharpness = Mathf.Max(0.001f, _parameters.CoughConsciousnessTransitionSharpness);

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

    private float CalculatePassiveFluidClearance(float fluidBurden)
    {
        float referenceFraction = Mathf.Clamp01(_parameters.PassiveFluidClearanceFractionPerRareTick);
        float scaledFraction = 1f - Mathf.Pow(1f - referenceFraction, ResolutionScale);
        return scaledFraction * fluidBurden;
    }

    private float CalculateChokingPressure(float fluidBurden)
    {
        float maximumPressure = _parameters.MaxChokingPressure;
        float sharpness = _parameters.ChokingPressureSharpness;
        float burdenPower = Mathf.Pow(fluidBurden, sharpness);
        return maximumPressure * burdenPower / (burdenPower + maximumPressure - 1f);
    }

    private float CalculateAirwayClearRecoveryFactor(float fluidBurden, float bleedRate, float coughStrength)
    {
        float lowFluidFactor = Mathf.InverseHillFactor(fluidBurden, _parameters.CoughRecoveryFluidHalfEffect, _parameters.CoughRecoveryFluidExponent);
        float lowBleedFactor = Mathf.InverseHillFactor(bleedRate, _parameters.CoughRecoveryBleedHalfEffect, _parameters.CoughRecoveryBleedExponent);
        return coughStrength * lowFluidFactor * lowBleedFactor;
    }

    private float CalculateSeverityChange(float currentSeverity, float chokingPressure, float fluidBurden, float bleedRate, float coughStrength)
    {
        float progression = IntervalDays * _parameters.ChokingSeverityProgressionPerDay * chokingPressure;

        float passiveRecoveryRate = _parameters.ChokingSeverityRecoveryPerDay / (1f + chokingPressure);
        float coughRecoveryFactor = CalculateAirwayClearRecoveryFactor(fluidBurden, bleedRate, coughStrength);
        float coughRecoveryRate = _parameters.CoughSeverityRecoveryPerDay * coughRecoveryFactor;
        float totalRecoveryRate = passiveRecoveryRate + coughRecoveryRate;

        float recoveryFraction = 1f - Mathf.Exp(-totalRecoveryRate * IntervalDays);
        float recovery = currentSeverity * recoveryFraction;
        return progression - recovery;
    }

    private static float NextMeanOneNoise(float amplitude = 1f)
    {
        amplitude = Mathf.Clamp01(amplitude);
        return 1f + Rand.Range(-amplitude, amplitude);
    }
}
