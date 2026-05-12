using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public abstract partial class HediffModifier_Genes : SecondaryHediffModifier
{
    [XmlBinding("genes")]
    public partial IReadOnlyList<GeneDef> Genes { get; }
}
