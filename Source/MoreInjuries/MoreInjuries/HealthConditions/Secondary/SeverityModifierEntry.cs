using MoreInjuries.HealthConditions.Secondary.Handlers;
using MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

/// <summary>
/// A single entry in <see cref="HediffModifier_SeverityModifiers_ModExtension.Modifiers"/>: pairs a
/// <see cref="SecondaryHediffModifier"/> with an <see cref="Apply"/> mode that controls how its
/// factor is combined with the current severity change.
/// </summary>
[XmlBindable]
public sealed partial class SeverityModifierEntry
{
    [XmlBinding("apply", NullableBackingField = true)]
    public partial SeverityModifierApplication Apply { get; }

    [XmlBinding("modifier")]
    public partial SecondaryHediffModifier Modifier { get; }

    /// <summary>
    /// Applies this entry's modifier to <paramref name="currentChange"/> according to <see cref="Apply"/>.
    /// </summary>
    public float ApplyTo(float currentChange, Hediff hediff, IHediffCompHandler compHandler)
    {
        float factor = Modifier.GetModifier(hediff, compHandler);
        return (Apply, currentChange) switch
        {
            (SeverityModifierApplication.OnChange, _) => currentChange * factor,
            (SeverityModifierApplication.OnIncrease, > 0f) => currentChange * factor,
            (SeverityModifierApplication.OnDecrease, < 0f) => currentChange * factor,
            (SeverityModifierApplication.SkewedIncrease, _) => currentChange + (factor * Math.Abs(currentChange)),
            (SeverityModifierApplication.SkewedDecrease, _) => currentChange - (factor * Math.Abs(currentChange)),
            _ => currentChange,
        };
    }
}
