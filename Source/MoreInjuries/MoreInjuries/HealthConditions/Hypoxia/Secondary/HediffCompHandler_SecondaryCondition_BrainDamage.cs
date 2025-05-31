using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.Hypoxia.Secondary;

public sealed class HediffCompHandler_SecondaryCondition_BrainDamage : HediffCompHandler_SecondaryCondition_TargetsBodyPart
{
    public override float Chance => MoreInjuriesMod.Settings.EnableNeuralDamage ? base.Chance : 0f;

    public override bool ShouldSkip(HediffComp_SecondaryCondition comp, float severityAdjustment)
    {
        return base.ShouldSkip(comp, severityAdjustment) 
            || comp.parent.IsTended() && Rand.Chance(MoreInjuriesMod.Settings.NeuralDamageChanceReductionFactor);
    }
}
