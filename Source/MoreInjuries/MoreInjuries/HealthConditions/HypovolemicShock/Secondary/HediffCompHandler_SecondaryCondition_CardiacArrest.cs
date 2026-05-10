﻿using MoreInjuries.Extensions;
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

        Pawn pawn = comp.parent.pawn;

        // Check for oxygen deficiency immunity from Deathless/Breathless genes FIRST
        // This prevents cardiac arrest entirely for oxygen-immune pawns
        if (pawn.HasOxygenDeficiencyImmunity())
        {
            return true;
        }

        // Don't apply cardiac arrest if pawn has an artificial heart
        if (HasArtificialHeart(pawn))
        {
            return true;
        }

        // if there is no blood loss, we don't apply cardiac arrest
        if (!pawn.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff? bloodLoss) || bloodLoss.Severity < Mathf.Epsilon)
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

    // Check if pawn has any artificial heart replacement (bionic, prosthetic, etc.)
    // Uses Hediff_AddedPart to properly distinguish artificial replacements from injuries/diseases
    private static bool HasArtificialHeart(Pawn pawn)
    {
        return pawn.health.hediffSet.hediffs.Any(hediff =>
            hediff.Part?.def == BodyPartDefOf.Heart &&
            hediff is Hediff_AddedPart);
    }
}