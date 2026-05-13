using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public sealed partial class HediffModifier_Genes_WhenPresent : HediffModifier_Genes_FactorBased
{
    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        Pawn pawn = hediff.pawn;
        float modifier = 1f;
        foreach (GenesFactorBasedModifierData geneModifier in GeneModifiers)
        {
            if (pawn.genes.HasActiveGene(geneModifier.GeneDef))
            {
                modifier *= geneModifier.Modifier;
            }
        }
        return modifier;
    }
}
