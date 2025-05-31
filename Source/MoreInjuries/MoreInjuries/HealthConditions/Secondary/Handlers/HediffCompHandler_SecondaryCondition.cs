using MoreInjuries.BuildIntrinsics;
using MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public abstract class HediffCompHandler_SecondaryCondition
{
    // don't rename this field. XML defs depend on this name
    protected readonly float? chance = null;
    // don't rename this field. XML defs depend on this name
    private readonly int tickInterval = GenTicks.TickRareInterval;
    // don't rename this field. XML defs depend on this name
    private readonly HediffMakerProperties? hediffMakerProps = default;

    public virtual float Chance => chance ?? 1f;

    public int TickInterval => tickInterval;

    public HediffMakerProperties? HediffMakerProps => hediffMakerProps;

    public virtual bool ShouldSkip(HediffComp_SecondaryCondition comp, float severityAdjustment)
    {
        if (!comp.Pawn.IsHashIntervalTick(TickInterval))
        {
            return true;
        }
        if (!Rand.Chance(comp.SeverityCurve.Evaluate(comp.parent.Severity)))
        {
            return true;
        }
        if (!Rand.Chance(Chance))
        {
            return true;
        }
        return false;
    }

    public abstract void Evaluate(HediffComp_SecondaryCondition comp, float severityAdjustment);
}