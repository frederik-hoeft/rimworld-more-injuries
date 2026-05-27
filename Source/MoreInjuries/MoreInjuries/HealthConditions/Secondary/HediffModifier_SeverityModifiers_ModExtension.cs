using MoreInjuries.HealthConditions.Secondary.Handlers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

/// <summary>
/// A <see cref="DefModExtension"/> attached to a <see cref="HediffDef"/> that lists severity modifiers,
/// each of which specifies how it should be applied to the current severity change
/// (see <see cref="SeverityModifierApplication"/>).
/// </summary>
[XmlBindable]
public sealed partial class HediffModifier_SeverityModifiers_ModExtension : DefModExtension
{
    [XmlBinding("modifiers")]
    public partial IReadOnlyList<SeverityModifierEntry> Modifiers { get; }

    /// <summary>
    /// Applies all configured modifiers to <paramref name="currentChange"/> in order and returns the result.
    /// </summary>
    public float ApplyTo(float currentChange, Hediff hediff, IHediffCompHandler compHandler)
    {
        foreach (SeverityModifierEntry entry in Modifiers)
        {
            currentChange = entry.ApplyTo(currentChange, hediff, compHandler);
        }
        return currentChange;
    }
}
