using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock.Secondary;

public sealed class HediffCompHandler_SecondaryCondition_Hypoxia : HediffCompHandler_SecondaryCondition_Tick
{
    public override float BaseChance => MoreInjuriesMod.Settings.OrganHypoxiaChance * base.BaseChance;

    public override bool ShouldSkip(HediffComp_SecondaryCondition comp)
    //=> 
    //base.ShouldSkip(comp) || comp.parent.IsTended() && Rand.Chance(MoreInjuriesMod.Settings.OrganHypoxiaChanceReductionFactor);
    {
        // Skip if pawn has oxygen-deficiency immunity (Deathless or Breathless genes)
        if (comp.parent.pawn.HasOxygenDeficiencyImmunity())
        {
            return true;
        }

        return base.ShouldSkip(comp) || comp.parent.IsTended() && Rand.Chance(MoreInjuriesMod.Settings.OrganHypoxiaChanceReductionFactor);
    }
}
