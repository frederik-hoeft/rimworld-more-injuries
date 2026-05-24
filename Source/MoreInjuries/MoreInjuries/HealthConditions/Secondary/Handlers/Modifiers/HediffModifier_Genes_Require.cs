using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_Genes_Require : HediffModifier_Genes
{
    protected override float GetModifier(Hediff hediff, Pawn_GeneTracker genes, IHediffCompHandler compHandler)
    {
        foreach (GeneDef geneDef in GeneDefs)
        {
            if (hediff.pawn.genes.HasActiveGene(geneDef))
            {
                return NoChange;
            }
        }
        return Disallow;
    }
}
