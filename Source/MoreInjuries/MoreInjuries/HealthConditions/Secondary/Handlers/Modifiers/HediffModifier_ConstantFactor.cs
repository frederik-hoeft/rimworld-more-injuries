using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffModifier_ConstantFactor : SecondaryHediffModifier
{
    // don't rename this field. XML defs depend on this name
    protected readonly float factor = default!;

    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler) => factor;
}