using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public sealed partial class HediffModifier_Genes_WhenPresent : HediffModifier_Genes_FactorBased
{
    [XmlBinding<bool>("stackDuplicates", defaultValue: false)]
    public partial bool StackDuplicates { get; }

    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        Pawn pawn = hediff.pawn;
        float modifier = 1f;
        foreach (GeneDef gen in Genes)
        {
            if (pawn.genes.HasActiveGene(gen))
            {
                modifier *= Factor;
                if (!StackDuplicates)
                {
                    break;
                }
            }
        }
        return modifier;
    }
}
