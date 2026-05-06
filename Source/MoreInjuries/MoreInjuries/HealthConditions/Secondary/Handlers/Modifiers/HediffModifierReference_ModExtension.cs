using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public sealed partial class HediffModifierReference_ModExtension : DefModExtension
{
    [XmlBinding("modifier")]
    public partial SecondaryHediffModifier Modifier { get; }
}
