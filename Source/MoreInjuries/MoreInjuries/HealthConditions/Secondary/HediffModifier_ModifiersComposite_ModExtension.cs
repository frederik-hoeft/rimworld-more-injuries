using MoreInjuries.HealthConditions.Secondary.Handlers;
using MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

[XmlBindable]
public abstract partial class HediffModifier_ModifiersComposite_ModExtension : DefModExtension, ISecondaryHediffModifier
{
    [XmlBinding("modifiers")]
    public partial IReadOnlyList<SecondaryHediffModifier> Modifiers { get; }

    public virtual float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        float result = 1f;
        foreach (SecondaryHediffModifier modifier in Modifiers)
        {
            result *= modifier.GetModifier(hediff, compHandler);
        }
        return result;
    }
}
