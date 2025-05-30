using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

public class HediffComp_SecondaryCondition : HediffComp
{
    private HediffCompProperties_SecondaryCondition Properties => (HediffCompProperties_SecondaryCondition)props;

    public SimpleCurve SeverityCurve => Properties.SeverityCurve;

    public override void CompPostTick(ref float severityAdjustment)
    {
        foreach (HediffCompHandler_SecondaryCondition handler in Properties.Handlers)
        {
            handler.Evaluate(this, severityAdjustment);
        }
    }
}