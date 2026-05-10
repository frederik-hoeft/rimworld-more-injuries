using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_Genes_Disallow : HediffModifier_Genes
{
    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        foreach (GeneDef gene in Genes)
        {
            if (hediff.pawn.genes.HasActiveGene(gene))
            {
                return 0f;
            }
        }
        return 1f;
    }
}
