using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_Genes_Disallow : HediffModifier_Genes
{
    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        foreach (GeneDef geneDef in GeneDefs)
        {
            if (hediff.pawn.genes.HasActiveGene(geneDef))
            {
                return 0f;
            }
        }
        return 1f;
    }
}
