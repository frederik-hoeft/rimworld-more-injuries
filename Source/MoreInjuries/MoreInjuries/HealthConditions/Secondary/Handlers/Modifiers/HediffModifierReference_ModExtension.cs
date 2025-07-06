using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class HediffModifierReference_ModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly SecondaryHediffModifier modifier = default!;

    public SecondaryHediffModifier Modifier => modifier ?? throw new InvalidOperationException("Modifier is not initialized. Please check your XML definition.");
}