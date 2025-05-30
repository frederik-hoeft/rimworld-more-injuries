using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock.Secondary;

public sealed class HediffCompHandler_SecondaryCondition_CardiacArrest : HediffCompHandler_SecondaryCondition_TargetsBodyPart
{
    protected override bool ShouldSkip(HediffComp_SecondaryCondition comp, float severityAdjustment)
    {
        if (base.ShouldSkip(comp, severityAdjustment) || !MoreInjuriesMod.Settings.EnableCardiacArrestOnHighBloodLoss)
        {
            return true;
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
        // Biotech integration: don't apply cardiac arrest if the pawn is deathresting, otherwise leads to infinite cardiac arrest
        if (ModLister.BiotechInstalled && comp.parent.pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest))
        {
            return true;
        }
        // continue with the evaluation
        return false;
    }

    protected override void PostApplyHediff(HediffComp_SecondaryCondition comp, Hediff hediff)
    {
        Pawn pawn = hediff.pawn;
        if (PawnUtility.ShouldSendNotificationAbout(pawn))
        {
            Find.LetterStack.ReceiveLetter("LetterHealthComplicationsLabel".Translate(pawn.LabelShort, hediff.LabelCap, pawn.Named("PAWN")).CapitalizeFirst(),
                "LetterHealthComplications".Translate(pawn.LabelShortCap, hediff.LabelCap, comp.parent.LabelCap, pawn.Named("PAWN")).CapitalizeFirst(),
                LetterDefOf.NegativeEvent, pawn);
        }
    }
}