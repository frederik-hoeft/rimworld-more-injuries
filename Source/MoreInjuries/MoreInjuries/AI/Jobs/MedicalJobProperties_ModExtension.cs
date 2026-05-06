using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs;

[XmlBindable]
public sealed partial class MedicalJobProperties_ModExtension : DefModExtension
{
    [XmlBinding<bool>("shouldEverBeTreatedFacingUp", defaultValue: false)]
    public partial bool ShouldEverBeTreatedFacingUp { get; }
}
