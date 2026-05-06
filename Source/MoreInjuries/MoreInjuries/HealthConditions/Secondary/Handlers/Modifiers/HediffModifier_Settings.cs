using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public abstract partial class HediffModifier_Settings : SecondaryHediffModifier
{
    [XmlBinding("key")]
    public partial string Key { get; }
}
