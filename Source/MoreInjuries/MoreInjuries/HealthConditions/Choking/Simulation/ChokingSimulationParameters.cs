namespace MoreInjuries.HealthConditions.Choking.Simulation;

internal readonly record struct ChokingSimulationParameters
{
    /// <summary>
    /// Normalized fluid burden gained per simulation interval at BleedRate = 1.
    /// Fluid burden is measured relative to the critical obstruction reference,
    /// where FluidBurden = 1 means the airway is critically obstructed.
    /// This is the main fluid-timescale dial.
    /// </summary>
    public float FluidGainPerBleedRateOneInterval { get; init; }

    /// <summary>
    /// Cough-triggered aspiration gain relative to direct bleeding fluid gain.
    /// For example, 0.25 means coughing while bleeding can add up to 25% of the
    /// direct bleeding contribution, scaled by cough strength and randomness.
    /// </summary>
    public float CoughAspirationGainRatio { get; init; }

    /// <summary>
    /// Maximum cough clearance relative to direct bleeding fluid gain at
    /// BleedRate = 1. For example, 0.5 means a strong cough can clear roughly
    /// half as much fluid as a BleedRate = 1 injury adds per interval, before
    /// scaling by available fluid, bleeding suppression, and randomness.
    /// </summary>
    public float CoughClearanceRatio { get; init; }

    /// <summary>
    /// Fraction of current fluid burden passively cleared per interval without
    /// effective coughing. This should remain tiny; it prevents permanent residue
    /// but should not rescue a heavily obstructed unconscious pawn quickly.
    /// </summary>
    public float PassiveFluidClearanceFractionPerInterval { get; init; }

    /// <summary>
    /// RimWorld consciousness level around which coughing should begin to collapse.
    /// This is not a hard cutoff. It calibrates the logistic cough curve so that
    /// cough strength is very low near the pawn unconsciousness threshold.
    /// Vanilla pawns generally become unconscious around 0.30 consciousness.
    /// </summary>
    public float CoughConsciousnessThreshold { get; init; }

    /// <summary>
    /// Relative random variation of direct bleeding fluid gain.
    /// This should usually be much smaller than cough-related variation, because
    /// most dramatic randomness should come from conscious struggling/coughing,
    /// not from unconscious passive suffocation.
    /// For example, 0.10 means direct bleeding varies by ±10% per interval.
    /// </summary>
    public float BleedingNoiseAmplitude { get; init; }

    /// <summary>
    /// Desired cough strength at <see cref="CoughConsciousnessThreshold" />.
    /// For example, 0.05 means a pawn at the unconsciousness threshold only has
    /// roughly 5% effective cough strength.
    /// </summary>
    public float CoughStrengthAtConsciousnessThreshold { get; init; }

    /// <summary>
    /// Sharpness of the logistic transition from ineffective to effective coughing.
    /// Higher values make cough strength rise more abruptly above the consciousness
    /// threshold; lower values make the transition more gradual.
    /// </summary>
    public float CoughConsciousnessTransitionSharpness { get; init; }

    /// <summary>
    /// Normalized fluid burden at which coughing has roughly half access to
    /// removable fluid. Since FluidBurden = 1 is critical obstruction, 0.05 means
    /// cough access reaches half effect at 5% of critical obstruction.
    /// </summary>
    public float CoughFluidHalfEffect { get; init; }

    /// <summary>
    /// Bleed rate at which ongoing bleeding suppresses cough clearance by roughly
    /// half. Lower values make even mild bleeding hard to overcome; higher values
    /// allow coughing to compensate for stronger bleeding.
    /// </summary>
    public float CoughSuppressionBleedHalfEffect { get; init; }

    /// <summary>
    /// Exponent controlling how sharply cough clearance fails as bleed rate rises.
    /// A value of 1 is gradual; values around 2 create a clearer transition from
    /// manageable minor bleeding to unmanageable moderate or heavy bleeding.
    /// </summary>
    public float CoughSuppressionBleedExponent { get; init; }

    /// <summary>
    /// Exponent used to convert normalized fluid burden into choking pressure.
    /// Higher values make low fluid burden relatively harmless while making high
    /// fluid burden escalate much more aggressively.
    /// </summary>
    public float ChokingPressureExponent { get; init; }

    /// <summary>
    /// Severity gained per game day at choking pressure = 1. This is the main
    /// lethality dial once fluid burden reaches the critical reference range.
    /// </summary>
    public float ChokingSeverityProgressionPerDay { get; init; }

    /// <summary>
    /// Severity recovered per game day when choking pressure is low. Recovery is
    /// dampened as choking pressure rises, so high airway obstruction still gets
    /// worse or remains dangerous.
    /// </summary>
    public float ChokingSeverityRecoveryPerDay { get; init; }

    public static ChokingSimulationParameters Default => new()
    {
        FluidGainPerBleedRateOneInterval = 0.16f,

        CoughAspirationGainRatio = 0.25f,
        CoughClearanceRatio = 0.50f,

        PassiveFluidClearanceFractionPerInterval = 0.0005f,

        // logistic curve parameters calibrated so that cough strength is around 5% at 30% consciousness,
        // approaches 0% as consciousness approaches 0%, and approaches 100% as consciousness approaches 100%
        CoughConsciousnessThreshold = 0.30f,
        CoughStrengthAtConsciousnessThreshold = 0.05f,
        CoughConsciousnessTransitionSharpness = 16f,

        BleedingNoiseAmplitude = 0.10f,

        CoughFluidHalfEffect = 0.05f,
        CoughSuppressionBleedHalfEffect = 0.35f,
        CoughSuppressionBleedExponent = 2f,

        ChokingPressureExponent = 4f,
        ChokingSeverityProgressionPerDay = 29.5f,
        ChokingSeverityRecoveryPerDay = 2.0f,
    };
}
