namespace MoreInjuries.HealthConditions.HeavyBleeding;

public static class BloodLossConstants
{
    /// <summary>
    /// The threshold for blood loss that indicates a "severe" blood loss condition.
    /// </summary>
    /// <remarks>
    /// This value is used in several places and corresponds to the volume of a full blood bag.
    /// We use .449 (rather than .45) to avoid triggering the medical emergency message for the "severe" condition (>= 0.45),
    /// for example, when a patient donates blood.
    /// Gameplay-wise, this value is pretty much the same as .45, but it avoids the message that would otherwise be displayed.
    /// </remarks>
    public const float BLOOD_LOSS_THRESHOLD = 0.449f;
}
