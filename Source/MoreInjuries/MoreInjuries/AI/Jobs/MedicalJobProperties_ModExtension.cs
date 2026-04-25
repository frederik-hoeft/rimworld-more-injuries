using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs;

[XmlSerializable]
public sealed partial class MedicalJobProperties_ModExtension : DefModExtension
{
    [XmlMember("shouldEverBeTreatedFacingUp")]
    public partial bool ShouldEverBeTreatedFacingUp { get; }
}
