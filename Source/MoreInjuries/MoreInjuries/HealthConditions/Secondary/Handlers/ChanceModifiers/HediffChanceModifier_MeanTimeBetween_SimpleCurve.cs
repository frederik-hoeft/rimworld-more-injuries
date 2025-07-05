using MoreInjuries.BuildIntrinsics;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffChanceModifier_MeanTimeBetween_SimpleCurve : HediffChanceModifier_MeanTimeBetween
{
    // don't rename this field. XML defs depend on this name
    protected readonly SimpleCurve mttfDaysBySeverity = default!;

    public override float GetModifier(HediffComp_SecondaryCondition comp, HediffCompHandler_SecondaryCondition compHandler)
    {
        if (mttfDaysBySeverity is null)
        {
            Logger.ConfigError($"{nameof(HediffChanceModifier_MeanTimeBetween_SimpleCurve)} is not properly initialized. Current MTTF curve is null. Cannot evaluate chance.");
            return 1f;
        }
        float mttfDays = mttfDaysBySeverity.Evaluate(comp.parent.Severity);
        return GetChanceFromMttf(mttfDays * GenDate.TicksPerDay, compHandler.TickInterval);
    }
}