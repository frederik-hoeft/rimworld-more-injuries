namespace MoreInjuries.HealthConditions.Secondary;

/// <summary>
/// Determines how a <see cref="Handlers.Modifiers.SecondaryHediffModifier"/>'s factor is applied to the
/// current severity change inside <see cref="HediffModifier_SeverityModifiers_ModExtension"/>.
/// </summary>
public enum SeverityModifierApplication
{
    /// <summary>Always multiply the current severity change by the factor.</summary>
    Always,

    /// <summary>Multiply the current severity change by the factor only when it is positive.</summary>
    OnIncrease,

    /// <summary>Multiply the current severity change by the factor only when it is negative.</summary>
    OnDecrease,

    /// <summary>Skew the current severity change upwards by adding <c>factor * abs(currentChange)</c>.</summary>
    IncreaseSkew,

    /// <summary>Skew the current severity change downwards by subtracting <c>factor * abs(currentChange)</c>.</summary>
    DecreaseSkew,
}
