using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock.Secondary;

public sealed class HediffCompHandler_SecondaryCondition_Hypoxia : HediffCompHandler_SecondaryCondition_TargetsBodyPart
{
    public override float Chance => MoreInjuriesMod.Settings.OrganHypoxiaChance * base.Chance;

    public override bool ShouldSkip(HediffComp_SecondaryCondition comp, float severityAdjustment)
    {
        return base.ShouldSkip(comp, severityAdjustment) 
            || comp.parent.IsTended() && Rand.Chance(MoreInjuriesMod.Settings.OrganHypoxiaChanceReductionFactor);
    }
}
