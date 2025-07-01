using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class BleedRateModifier_ModExtension : DefModExtension
{
    // do not rename this field. XML defs depend on this name
    private readonly BleedRateModifier modifier = default!;

    public BleedRateModifier Modifier => modifier;
}