using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffModifier_SimpleCurve : SecondaryHediffModifier
{
    // don't rename this field. XML defs depend on this name
    protected readonly SimpleCurve severityCurve = default!;

    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        if (severityCurve is null)
        {
            Logger.ConfigError($"{nameof(HediffModifier_SimpleCurve)} is not properly initialized. Current MTTF curve is null. Cannot evaluate chance.");
            return 1f;
        }
        return severityCurve.Evaluate(hediff.Severity);
    }
}