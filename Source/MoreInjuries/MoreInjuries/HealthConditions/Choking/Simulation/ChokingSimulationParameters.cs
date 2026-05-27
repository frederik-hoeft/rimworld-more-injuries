namespace MoreInjuries.HealthConditions.Choking.Simulation;

internal readonly record struct ChokingSimulationParameters
{
    /// <summary>
    /// Normalized fluid burden gained each rare tick at BleedRate = 1.
    /// Runtime tick interval changes scale this value by elapsed
    /// time, so lowering the simulation tick interval improves resolution without
    /// increasing total fluid accumulation.
    /// </summary>
    public required float FluidGainPerBleedRateRareTick { get; init; }

    /// <summary>
    /// Maximum normalized fluid burden retained by the model. FluidBurden = 1 is
    /// critical reference obstruction, not the physical maximum. Values above 1
    /// represent severe aspiration, but this cap prevents unbounded accumulation
    /// from dominating future recovery/rescue behavior.
    /// </summary>
    public required float MaximumFluidBurden { get; init; }

    /// <summary>
    /// Cough-triggered aspiration gain relative to direct bleeding fluid gain.
    /// For example, 0.25 means coughing while bleeding can add up to 25% of the
    /// direct bleeding contribution, scaled by cough strength and randomness.
    /// </summary>
    public required float CoughAspirationGainRatio { get; init; }

    /// <summary>
    /// Maximum cough clearance relative to direct bleeding fluid gain at
    /// BleedRate = 1. For example, 0.5 means a strong cough can clear roughly
    /// half as much fluid as a BleedRate = 1 injury adds per interval, before
    /// scaling by available fluid, bleeding suppression, and randomness.
    /// </summary>
    public required float CoughClearanceRatio { get; init; }

    /// <summary>
    /// Fraction of current fluid burden passively cleared over the model reference
    /// interval. Runtime tick interval changes scale this exponentially so passive
    /// clearance remains mostly independent of simulation resolution.
    /// </summary>
    public required float PassiveFluidClearanceFractionPerRareTick { get; init; }

    /// <summary>
    /// RimWorld consciousness level around which coughing should begin to collapse.
    /// This is not a hard cutoff. It calibrates the logistic cough curve so that
    /// cough strength is very low near the pawn unconsciousness threshold.
    /// Vanilla pawns generally become unconscious around 0.30 consciousness.
    /// </summary>
    public required float CoughConsciousnessThreshold { get; init; }

    /// <summary>
    /// Relative random variation of direct bleeding fluid gain.
    /// This should usually be much smaller than cough-related variation, because
    /// most dramatic randomness should come from conscious struggling/coughing,
    /// not from unconscious passive suffocation.
    /// For example, 0.10 means direct bleeding varies by ±10% per interval.
    /// </summary>
    public required float BleedingNoiseAmplitude { get; init; }

    /// <summary>
    /// Desired cough strength at <see cref="CoughConsciousnessThreshold" />.
    /// For example, 0.05 means a pawn at the unconsciousness threshold only has
    /// roughly 5% effective cough strength.
    /// </summary>
    public required float CoughStrengthAtConsciousnessThreshold { get; init; }

    /// <summary>
    /// Sharpness of the logistic transition from ineffective to effective coughing.
    /// Higher values make cough strength rise more abruptly above the consciousness
    /// threshold; lower values make the transition more gradual.
    /// </summary>
    public required float CoughConsciousnessTransitionSharpness { get; init; }

    /// <summary>
    /// Normalized fluid burden at which coughing has roughly half access to
    /// removable fluid. Since FluidBurden = 1 is critical obstruction, 0.05 means
    /// cough access reaches half effect at 5% of critical obstruction.
    /// </summary>
    public required float CoughFluidHalfEffect { get; init; }

    /// <summary>
    /// Bleed rate at which ongoing bleeding suppresses cough clearance by roughly
    /// half. Lower values make even mild bleeding hard to overcome; higher values
    /// allow coughing to compensate for stronger bleeding.
    /// </summary>
    public required float CoughSuppressionBleedHalfEffect { get; init; }

    /// <summary>
    /// Exponent controlling how sharply cough clearance fails as bleed rate rises.
    /// A value of 1 is gradual; values around 2 create a clearer transition from
    /// manageable minor bleeding to unmanageable moderate or heavy bleeding.
    /// </summary>
    public required float CoughSuppressionBleedExponent { get; init; }

    /// <summary>
    /// Additional severity recovery rate per game day when the pawn is conscious,
    /// coughing effectively, not bleeding, and has little remaining airway fluid.
    /// This models rapid clearing of the choking condition once the airway is
    /// effectively clear.
    /// </summary>
    public required float CoughSeverityRecoveryPerDay { get; init; }

    /// <summary>
    /// Fluid burden at which cough-assisted severity recovery is reduced by half.
    /// Low values make rapid recovery only happen when the airway is nearly clear.
    /// Since FluidBurden = 1 is critical obstruction, 0.02 means 2% of critical
    /// obstruction.
    /// </summary>
    public required float CoughRecoveryFluidHalfEffect { get; init; }

    /// <summary>
    /// Exponent controlling how sharply cough-assisted severity recovery shuts down
    /// as remaining fluid burden rises.
    /// </summary>
    public required float CoughRecoveryFluidExponent { get; init; }

    /// <summary>
    /// Bleed rate at which cough-assisted severity recovery is reduced by half.
    /// This prevents rapid recovery while the source is still actively bleeding.
    /// </summary>
    public required float CoughRecoveryBleedHalfEffect { get; init; }

    /// <summary>
    /// Exponent controlling how sharply cough-assisted severity recovery shuts down
    /// as active bleeding rises.
    /// </summary>
    public required float CoughRecoveryBleedExponent { get; init; }

    /// <summary>
    /// Maximum effective choking pressure used for severity progression. This keeps
    /// fluid burden above the critical reference point dangerous without allowing
    /// the exponential pressure curve to create multi-severity instant-death jumps.
    /// </summary>
    public required float MaxChokingPressure { get; init; }

    /// <summary>
    /// Sharpness of the curve controlling how rapidly choking severity ramps
    /// as choking pressure approaches the critical reference point. Higher values make
    /// severity rise more abruptly as choking pressure increases; lower values make
    /// the transition more gradual.
    /// </summary>
    public required float ChokingPressureSharpness { get; init; }

    /// <summary>
    /// Severity gained per game day at choking pressure = 1. This is the main
    /// lethality dial once fluid burden reaches the critical reference range.
    /// </summary>
    public required float ChokingSeverityProgressionPerDay { get; init; }

    /// <summary>
    /// Severity recovered per game day when choking pressure is low. Recovery is
    /// dampened as choking pressure rises, so high airway obstruction still gets
    /// worse or remains dangerous.
    /// </summary>
    public required float ChokingSeverityRecoveryPerDay { get; init; }

    public static ChokingSimulationParameters Default => new()
    {
        FluidGainPerBleedRateRareTick = 0.05f,
        MaximumFluidBurden = 3f,

        CoughAspirationGainRatio = 0.25f,
        CoughClearanceRatio = 0.50f,

        PassiveFluidClearanceFractionPerRareTick = 0.0002f,

        // logistic curve parameters calibrated so that cough strength is around 5% at 30% consciousness,
        // approaches 0% as consciousness approaches 0%, and approaches 100% as consciousness approaches 100%
        CoughConsciousnessThreshold = 0.30f,
        CoughStrengthAtConsciousnessThreshold = 0.05f,
        CoughConsciousnessTransitionSharpness = 16f,

        BleedingNoiseAmplitude = 0.10f,

        CoughFluidHalfEffect = 0.05f,
        CoughSuppressionBleedHalfEffect = 0.35f,
        CoughSuppressionBleedExponent = 2f,

        MaxChokingPressure = 2f,
        ChokingPressureSharpness = 4f,
        ChokingSeverityProgressionPerDay = 8f,
        ChokingSeverityRecoveryPerDay = 2.0f,

        CoughSeverityRecoveryPerDay = 24f,
        CoughRecoveryFluidHalfEffect = 0.02f,
        CoughRecoveryFluidExponent = 2f,
        CoughRecoveryBleedHalfEffect = 0.05f,
        CoughRecoveryBleedExponent = 2f,
    };
}
