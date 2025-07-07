using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class HediffModifierReference_ModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly SecondaryHediffModifier modifier = default!;

    public SecondaryHediffModifier Modifier => modifier ?? throw new InvalidOperationException("Modifier is not initialized. Please check your XML definition.");
}