using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public abstract partial class HediffModifier_Genes : SecondaryHediffModifier
{
    [XmlBinding("geneDefs")]
    public partial IReadOnlyList<GeneDef> GeneDefs { get; }
}
