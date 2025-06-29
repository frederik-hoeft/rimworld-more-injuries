using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class HediffChanceModifier_LinkedHediff_MeanTimeBetween_SimpleCurve : HediffChanceModifier_MeanTimeBetween_SimpleCurve
{
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef hediffDef = default!;

    public override float GetModifier(HediffComp_SecondaryCondition comp, HediffCompHandler_SecondaryCondition compHandler)
    {
        if (mttfDaysBySeverity is null || hediffDef is null)
        {
            Logger.ConfigError($"{nameof(HediffChanceModifier_MeanTimeBetween_SimpleCurve)} is not properly initialized. Current MTTF curve is null. Cannot evaluate chance.");
            return 1f;
        }
        if (!comp.parent.pawn.health.hediffSet.TryGetHediff(hediffDef, out Hediff? hediff))
        {
            return 1f;
        }
        float mttf = mttfDaysBySeverity.Evaluate(hediff.Severity);
        return GetChanceFromMttf(mttf, compHandler.TickInterval);
    }
}