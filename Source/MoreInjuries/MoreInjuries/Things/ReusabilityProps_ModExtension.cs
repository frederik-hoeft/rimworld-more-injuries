using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.Things;

[XmlSerializable]
public partial class ReusabilityProps_ModExtension : DefModExtension
{
    [XmlMember("destroyChance")]
    public partial float DestroyChance { get; }
}