using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public abstract partial class HediffModifier_Genes_FactorBased : HediffModifier_Genes
{
    [XmlBinding("factor", NullableBackingField = true)]
    public partial float Factor { get; }
}
