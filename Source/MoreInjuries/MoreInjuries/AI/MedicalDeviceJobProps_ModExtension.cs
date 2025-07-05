using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.AI;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class MedicalDeviceJobProps_ModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly bool shouldEverBeTreatedFacingUp = false;

    public bool ShouldEverBeTreatedFacingUp => shouldEverBeTreatedFacingUp;
}
