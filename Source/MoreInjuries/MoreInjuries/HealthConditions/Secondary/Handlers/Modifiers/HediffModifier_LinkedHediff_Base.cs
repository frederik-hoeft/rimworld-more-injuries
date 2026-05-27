using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public abstract partial class HediffModifier_LinkedHediff_Base : SecondaryHediffModifier
{
    [XmlBinding("hediffDef")]
    public partial HediffDef HediffDef { get; }
}
