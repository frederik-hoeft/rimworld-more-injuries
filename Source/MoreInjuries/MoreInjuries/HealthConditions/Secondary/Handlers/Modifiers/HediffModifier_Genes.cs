using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public abstract partial class HediffModifier_Genes : SecondaryHediffModifier
{
    [XmlBinding("geneDefs")]
    public partial IReadOnlyList<GeneDef> GeneDefs { get; }

    protected abstract float GetModifier(Hediff hediff, Pawn_GeneTracker genes, IHediffCompHandler compHandler);

    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        if (hediff.pawn.genes is not { } genes)
        {
            return NoChange;
        }
        return GetModifier(hediff, genes, compHandler);
    }
}
