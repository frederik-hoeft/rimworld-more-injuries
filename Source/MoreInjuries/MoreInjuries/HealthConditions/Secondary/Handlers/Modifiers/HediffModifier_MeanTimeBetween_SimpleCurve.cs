using MoreInjuries.BuildIntrinsics;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffModifier_MeanTimeBetween_SimpleCurve : HediffModifier_MeanTimeBetween
{
    // don't rename this field. XML defs depend on this name
    protected readonly SimpleCurve mttfDaysBySeverity = default!;

    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        if (mttfDaysBySeverity is null)
        {
            Logger.ConfigError($"{nameof(HediffModifier_MeanTimeBetween_SimpleCurve)} is not properly initialized. Current MTTF curve is null. Cannot evaluate chance.");
            return 1f;
        }
        float mttfDays = mttfDaysBySeverity.Evaluate(hediff.Severity);
        return GetChanceFromMttf(mttfDays * GenDate.TicksPerDay, compHandler.TickInterval);
    }
}