using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public abstract partial class HediffModifier_Genes : SecondaryHediffModifier
{
    [XmlBinding("genes", DecorateWith = typeof(MayRequireBiotechAttribute))]
    public partial IReadOnlyList<GeneDef> Genes { get; }
}
