using MoreInjuries.HealthConditions.Secondary.Handlers;
using MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

[XmlBindable]
public sealed partial class HediffModifier_InboundCauseComposite_ModExtension : DefModExtension, ISecondaryHediffModifier
{
    [XmlBinding("modifiers")]
    public partial IReadOnlyList<SecondaryHediffModifier> Modifiers { get; }

    public float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        float result = 1f;
        foreach (SecondaryHediffModifier modifier in Modifiers)
        {
            result *= modifier.GetModifier(hediff, compHandler);
        }
        return result;
    }
}
