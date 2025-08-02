using MoreInjuries.Roslyn.Future.ThrowHelpers;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
public abstract class HediffModifier_Settings : SecondaryHediffModifier
{
    // don't rename this field. XML defs depend on this name
    private readonly string? key = null;

    public string Key
    {
        get
        {
            Throw.InvalidOperationException.IfNullOrEmpty(this, key); 
            return key;
        }
    }
}
