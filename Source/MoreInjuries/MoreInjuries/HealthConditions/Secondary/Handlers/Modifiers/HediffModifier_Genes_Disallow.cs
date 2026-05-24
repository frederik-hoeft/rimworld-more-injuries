using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_Genes_Disallow : HediffModifier_Genes
{
    protected override float GetModifier(Hediff hediff, Pawn_GeneTracker genes, IHediffCompHandler compHandler)
    {
        foreach (GeneDef geneDef in GeneDefs)
        {
            if (genes.HasActiveGene(geneDef))
            {
                return Disallow;
            }
        }
        return NoChange;
    }
}
