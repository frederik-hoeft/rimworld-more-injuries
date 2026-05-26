using MoreInjuries.HealthConditions.Secondary.Handlers;
using MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

/// <summary>
/// A single entry in <see cref="HediffModifier_SeverityModifiers_ModExtension.Modifiers"/>: pairs a
/// <see cref="SecondaryHediffModifier"/> with an application <see cref="Mode"/> that controls how its
/// factor is combined with the current severity change.
/// </summary>
[XmlBindable]
public sealed partial class SeverityModifierEntry
{
    [XmlBinding<SeverityModifierApplication>("mode", SeverityModifierApplication.Always)]
    public partial SeverityModifierApplication Mode { get; }

    [XmlBinding("modifier")]
    public partial SecondaryHediffModifier Modifier { get; }

    /// <summary>
    /// Applies this entry's modifier to <paramref name="currentChange"/> according to <see cref="Mode"/>.
    /// </summary>
    public float ApplyTo(float currentChange, Hediff hediff, IHediffCompHandler compHandler)
    {
        float factor = Modifier.GetModifier(hediff, compHandler);
        return Mode switch
        {
            SeverityModifierApplication.Always => currentChange * factor,
            SeverityModifierApplication.OnIncrease => currentChange > 0f ? currentChange * factor : currentChange,
            SeverityModifierApplication.OnDecrease => currentChange < 0f ? currentChange * factor : currentChange,
            SeverityModifierApplication.IncreaseSkew => currentChange + factor * Math.Abs(currentChange),
            SeverityModifierApplication.DecreaseSkew => currentChange - factor * Math.Abs(currentChange),
            _ => currentChange,
        };
    }
}
