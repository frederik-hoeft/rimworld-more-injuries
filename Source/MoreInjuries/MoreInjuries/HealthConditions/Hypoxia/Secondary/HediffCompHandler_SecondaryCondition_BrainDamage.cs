﻿using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.Hypoxia.Secondary;

public sealed class HediffCompHandler_SecondaryCondition_BrainDamage : HediffCompHandler_SecondaryCondition_Tick
{
    public override float BaseChance => MoreInjuriesMod.Settings.EnableNeuralDamage ? base.BaseChance : 0f;

    public override bool ShouldSkip(HediffComp_SecondaryCondition comp)
    {
        // Skip if pawn has oxygen-deficiency immunity (Deathless or Breathless genes)
        if (comp.parent.pawn.HasOxygenDeficiencyImmunity())
        {
            return true;
        }

        return base.ShouldSkip(comp) || comp.parent.IsTended() && Rand.Chance(MoreInjuriesMod.Settings.NeuralDamageChanceReductionFactor);
    }
}
