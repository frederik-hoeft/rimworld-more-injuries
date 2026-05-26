using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

public class HediffComp_SecondaryCondition : HediffComp
{
    private HediffCompProperties_SecondaryCondition Properties => (HediffCompProperties_SecondaryCondition)props;

    public SimpleCurve? SeverityCurve => Properties.SeverityCurve;

    protected virtual bool ShouldSkip()
    {
        if (parent.pawn is null or { Dead: true })
        {
            return true;
        }
        if (SeverityCurve is { } severityCurve && !Rand.Chance(severityCurve.Evaluate(parent.Severity)))
        {
            return true;
        }
        return false;
    }

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        if (!ShouldSkip())
        {
            foreach (IHediffComp_SecondaryCondition_PostMakeHandler handler in Properties.PostMakeHandlers)
            {
                handler.Handle(this);
            }
        }
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (!ShouldSkip())
        {
            foreach (IHediffComp_SecondaryCondition_TickHandler handler in Properties.TickHandlers)
            {
                if (Pawn.IsHashIntervalTick(handler.TickInterval))
                {
                    handler.Handle(this);
                }
            }
        }
    }
}
