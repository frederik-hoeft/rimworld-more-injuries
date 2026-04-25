using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs;

[XmlSerializable]
public sealed partial class MedicalJobProperties_ModExtension : DefModExtension
{
    [XmlMember<bool>("shouldEverBeTreatedFacingUp", defaultValue: false)]
    public partial bool ShouldEverBeTreatedFacingUp { get; }
}
