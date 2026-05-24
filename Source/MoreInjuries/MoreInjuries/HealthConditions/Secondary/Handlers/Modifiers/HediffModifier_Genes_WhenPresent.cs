using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public sealed partial class HediffModifier_Genes_WhenPresent : HediffModifier_Genes_FactorBased
{
    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        if (hediff.pawn.genes is not { } genes)
        {
            return NoChange;
        }
        float modifier = NoChange;
        foreach (GenesFactorBasedModifierData geneModifier in GeneModifiers)
        {
            if (genes.HasActiveGene(geneModifier.GeneDef))
            {
                modifier *= geneModifier.Modifier;
            }
        }
        return modifier;
    }
}
