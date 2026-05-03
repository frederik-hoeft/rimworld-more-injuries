using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using MoreInjuries.Things.Modifiers;
using Verse;

namespace MoreInjuries.Things;

[XmlSerializable]
public partial class ReusabilityProps_ModExtension : DefModExtension
{
    [XmlMember("destroyChanceModifier")]
    public partial ThingModifier DestroyChanceModifier { get; }
}
