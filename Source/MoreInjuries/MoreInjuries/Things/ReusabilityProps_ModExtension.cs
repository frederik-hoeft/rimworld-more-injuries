using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using MoreInjuries.Things.Modifiers;
using Verse;

namespace MoreInjuries.Things;

[XmlBindable]
public partial class ReusabilityProps_ModExtension : DefModExtension
{
    [XmlBinding("destroyChanceModifier")]
    public partial ThingModifier DestroyChanceModifier { get; }
}
