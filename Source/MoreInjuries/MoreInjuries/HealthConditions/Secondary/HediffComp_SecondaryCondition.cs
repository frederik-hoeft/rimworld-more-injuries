using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

public class HediffComp_SecondaryCondition : HediffComp
{
    private HediffCompProperties_SecondaryCondition Properties => (HediffCompProperties_SecondaryCondition)props;

    public SimpleCurve? SeverityCurve => Properties.SeverityCurve;

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (parent.pawn is not { Dead: false })
        {
            return;
        }
        foreach (HediffCompHandler_SecondaryCondition handler in Properties.Handlers)
        {
            handler.Evaluate(this, severityAdjustment);
        }
    }
}