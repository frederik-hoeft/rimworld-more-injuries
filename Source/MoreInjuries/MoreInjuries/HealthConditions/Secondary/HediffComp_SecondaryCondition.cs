using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

public class HediffComp_SecondaryCondition : HediffComp
{
    private HediffCompProperties_SecondaryCondition Properties => (HediffCompProperties_SecondaryCondition)props;

    public SimpleCurve? SeverityCurve => Properties.SeverityCurve;

    protected bool ValidState => parent.pawn is { Dead: false };

    protected virtual bool ShouldSkip() => SeverityCurve is { } severityCurve && !Rand.Chance(severityCurve.Evaluate(parent.Severity));

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        if (ValidState && !ShouldSkip())
        {
            foreach (IHediffComp_SecondaryCondition_PostMakeHandler handler in Properties.PostMakeHandlers)
            {
                handler.Handle(this);
            }
        }
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (ValidState)
        {
            // we move the ShouldSkip check inside the loop to ensure that we don't evaluate the curve even if there are no handlers to process this tick
            bool processedHandler = false;
            foreach (IHediffComp_SecondaryCondition_TickHandler handler in Properties.TickHandlers)
            {
                if (Pawn.IsHashIntervalTick(handler.TickInterval))
                {
                    if (!processedHandler && ShouldSkip())
                    {
                        return;
                    }
                    handler.Handle(this);
                    processedHandler = true;
                }
            }
        }
    }
}
