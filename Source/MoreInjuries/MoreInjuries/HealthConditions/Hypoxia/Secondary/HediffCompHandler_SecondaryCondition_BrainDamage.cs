using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.Hypoxia.Secondary;

public sealed class HediffCompHandler_SecondaryCondition_BrainDamage : HediffCompHandler_SecondaryCondition
{
    public override float BaseChance => MoreInjuriesMod.Settings.EnableNeuralDamage ? base.BaseChance : 0f;

    public override bool ShouldSkip(HediffComp_SecondaryCondition comp, float severityAdjustment)
    {
        return base.ShouldSkip(comp, severityAdjustment) 
            || comp.parent.IsTended() && Rand.Chance(MoreInjuriesMod.Settings.NeuralDamageChanceReductionFactor);
    }
}
