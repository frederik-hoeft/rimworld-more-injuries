using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffModifier_LinkedHediff_SimpleCurve : SecondaryHediffModifier
{
    // don't rename this field. XML defs depend on this name
    private readonly SimpleCurve severityCurve = default!;
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef hediffDef = default!;

    /// <inheritdoc />
    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        if (severityCurve is null || hediffDef is null)
        {
            Logger.ConfigError($"{nameof(HediffModifier_LinkedHediff_SimpleCurve)} is not properly initialized. Current severity curve is null. Cannot evaluate chance.");
            return 1f;
        }
        if (hediff.pawn.health.hediffSet.TryGetHediff(hediffDef, out Hediff? linkedHediff))
        {
            // if the hediff exists, we evaluate the chance based on the severity curve
            return severityCurve.Evaluate(linkedHediff.Severity);
        }
        // if the hediff does not exist, we return the base chance
        return 1f;
    }
}