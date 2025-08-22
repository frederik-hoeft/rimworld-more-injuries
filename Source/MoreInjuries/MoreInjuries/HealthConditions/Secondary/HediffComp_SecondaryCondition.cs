using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

public class HediffComp_SecondaryCondition : HediffComp
{
    private HediffCompProperties_SecondaryCondition Properties => (HediffCompProperties_SecondaryCondition)props;

    public SimpleCurve? SeverityCurve => Properties.SeverityCurve;

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        if (parent.pawn is not { Dead: false })
        {
            return;
        }
        foreach (IHediffComp_SecondaryCondition_PostMakeHandler handler in Properties.PostMakeHandlers)
        {
            handler.PostMake(this);
        }
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (parent.pawn is not { Dead: false })
        {
            return;
        }
        foreach (IHediffComp_SecondaryCondition_TickHandler handler in Properties.TickHandlers)
        {
            handler.Tick(this);
        }
    }
}