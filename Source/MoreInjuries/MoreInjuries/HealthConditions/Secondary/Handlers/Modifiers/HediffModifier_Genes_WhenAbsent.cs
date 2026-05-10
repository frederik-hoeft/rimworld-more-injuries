using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_Genes_WhenAbsent : HediffModifier_Genes_FactorBased
{
    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        Pawn pawn = hediff.pawn;
        float modifier = Factor;
        foreach (GeneDef gen in Genes)
        {
            if (pawn.genes.HasActiveGene(gen))
            {
                modifier = 1f;
                break;
            }
        }
        return modifier;
    }
}
