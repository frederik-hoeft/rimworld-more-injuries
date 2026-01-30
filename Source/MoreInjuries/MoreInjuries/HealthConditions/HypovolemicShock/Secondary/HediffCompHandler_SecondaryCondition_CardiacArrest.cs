using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock.Secondary;

public sealed class HediffCompHandler_SecondaryCondition_CardiacArrest : HediffCompHandler_SecondaryCondition_Tick
{
    public override bool ShouldSkip(HediffComp_SecondaryCondition comp)
    {
        if (base.ShouldSkip(comp) || !MoreInjuriesMod.Settings.EnableCardiacArrestOnHighBloodLoss)
        {
            return true;
        }

        // Don't apply cardiac arrest if pawn has an artificial heart
        if (HasArtificialHeart(comp.parent.pawn))
        {
            return true; // Skip - pawn has artificial heart
        }

        // if there is no blood loss, we don't apply cardiac arrest
        if (!comp.parent.pawn.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff? bloodLoss) || bloodLoss.Severity < Mathf.Epsilon)
        {
            return true;
        }
        // cardiac arrest chance is higher for higher blood loss
        float cardiacArrestChance = MoreInjuriesMod.Settings.CardiacArrestChanceOnHighBloodLoss * bloodLoss.Severity / 0.8f;
        if (!Rand.Chance(cardiacArrestChance))
        {
            return true;
        }
        // continue with the evaluation
        return false;
    }

    private static bool HasArtificialHeart(Pawn pawn)
    {
        // Check if pawn has an artificial heart
        return pawn.health.hediffSet.hediffs.Any(hediff =>
            hediff.Part?.def == BodyPartDefOf.Heart && 
            hediff.def.addedPartProps is not null);
    }
}