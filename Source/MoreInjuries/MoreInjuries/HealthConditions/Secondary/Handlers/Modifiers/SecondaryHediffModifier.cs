using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public abstract class SecondaryHediffModifier : ISecondaryHediffModifier
{
    /// <summary>
    /// Indicates that the modifier does not change the chance of the secondary hediff occurring.
    /// This is useful for modifiers that only apply under certain conditions.
    /// </summary>
    protected static float NoChange => 1f;

    /// <summary>
    /// Indicates that the modifier completely prevents the secondary hediff from occurring.
    /// </summary>
    protected static float Disallow => 0f;

    public abstract float GetModifier(Hediff hediff, IHediffCompHandler compHandler);
}
