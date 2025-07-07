using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class HediffModifier_LinkedHediff_MeanTimeBetween_SimpleCurve : HediffModifier_MeanTimeBetween_SimpleCurve
{
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef hediffDef = default!;

    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        if (mttfDaysBySeverity is null || hediffDef is null)
        {
            Logger.ConfigError($"{nameof(HediffModifier_MeanTimeBetween_SimpleCurve)} is not properly initialized. Current MTTF curve is null. Cannot evaluate chance.");
            return 1f;
        }
        if (!hediff.pawn.health.hediffSet.TryGetHediff(hediffDef, out Hediff? linkedHediff))
        {
            return 1f;
        }
        float mttf = mttfDaysBySeverity.Evaluate(linkedHediff.Severity);
        return GetChanceFromMttf(mttf, compHandler.TickInterval);
    }
}