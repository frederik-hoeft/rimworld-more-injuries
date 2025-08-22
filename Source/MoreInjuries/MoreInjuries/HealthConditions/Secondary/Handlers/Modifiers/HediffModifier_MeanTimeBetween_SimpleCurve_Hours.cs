using MoreInjuries.Roslyn.Future.ThrowHelpers;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffModifier_MeanTimeBetween_SimpleCurve_Hours : HediffModifier_MeanTimeBetween
{
    // don't rename this field. XML defs depend on this name
    protected readonly SimpleCurve mttfHoursBySeverity = default!;

    public override float GetModifier(Hediff hediff, IHediffComp_TickHandler compHandler)
    {
        Throw.InvalidOperationException.IfNull(this, mttfHoursBySeverity);
        float mttfHours = mttfHoursBySeverity.Evaluate(hediff.Severity);
        return GetChanceFromMttf(mttfHours * GenDate.TicksPerHour, compHandler.TickInterval);
    }
}