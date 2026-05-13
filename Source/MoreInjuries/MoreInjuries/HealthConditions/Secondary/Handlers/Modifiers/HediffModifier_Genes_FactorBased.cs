using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public abstract partial class HediffModifier_Genes_FactorBased : SecondaryHediffModifier
{
    [XmlBinding("geneModifiers")]
    public partial IReadOnlyList<GenesFactorBasedModifierData> GeneModifiers { get; }
}

[XmlBindable]
public sealed partial class GenesFactorBasedModifierData
{
    [XmlBinding("modifier", NullableBackingField = true)]
    public partial float Modifier { get; }

    [XmlBinding("geneDef")]
    public partial GeneDef GeneDef { get; }
}
