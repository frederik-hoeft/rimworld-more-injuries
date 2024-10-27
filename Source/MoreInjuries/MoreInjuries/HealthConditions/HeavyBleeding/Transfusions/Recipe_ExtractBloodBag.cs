using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

using static BloodLossConstants;

public class Recipe_ExtractBloodBag : Recipe_Surgery
{
    private const float BLOOD_LOSS_SEVERITY = BLOOD_LOSS_THRESHOLD;
    private const float MIN_BLOODLOSS_FOR_FAILURE = BLOOD_LOSS_THRESHOLD;

    public override bool AvailableOnNow(Thing thing, BodyPartRecord? part = null)
    {
        if (thing is not Pawn pawn)
        {
            return false;
        }
        if (ModLister.BiotechInstalled && pawn.genes.HasActiveGene(GeneDefOf.Hemogenic))
        {
            return false;
        }
        return pawn.health.CanBleed && base.AvailableOnNow(thing, part);
    }

    public override AcceptanceReport AvailableReport(Thing thing, BodyPartRecord? part = null)
    {
        if (thing is Pawn pawn && pawn.DevelopmentalStage.Baby())
        {
            return "too small";
        }
        return base.AvailableReport(thing, part);
    }

    public override bool CompletableEver(Pawn surgeryTarget) => base.CompletableEver(surgeryTarget) && PawnHasEnoughBloodForExtraction(surgeryTarget);

    public override void CheckForWarnings(Pawn medPawn)
    {
        base.CheckForWarnings(medPawn);
        if (PawnHasEnoughBloodForExtraction(medPawn))
        {
            return;
        }

        Messages.Message($"{medPawn.Name} doesn't have enough blood to safely donate blood.", medPawn, MessageTypeDefOf.NeutralEvent, false);
    }

    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        if (!PawnHasEnoughBloodForExtraction(pawn))
        {
            Messages.Message($"{pawn.Name} did not have enough blood to safely donate blood.", pawn, MessageTypeDefOf.NeutralEvent);
            return;
        }
        Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, pawn);
        hediff.Severity = BLOOD_LOSS_SEVERITY;
        pawn.health.AddHediff(hediff);
        OnSurgerySuccess(pawn, part, billDoer, ingredients, bill);
        if (!IsViolationOnPawn(pawn, part, Faction.OfPlayer))
        {
            return;
        }

        ReportViolation(pawn, billDoer, pawn.HomeFaction, -1, KnownHistoryEventDefOf.ExtractedWholeBloodBag);
    }

    protected override void OnSurgerySuccess(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        if (GenPlace.TryPlaceThing(ThingMaker.MakeThing(KnownThingDefOf.WholeBloodBag), pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near))
        {
            return;
        }
        // TODO: make positive thought if colonist

        Logger.Error($"Could not drop blood bag near {pawn.PositionHeld}");
    }

    private bool PawnHasEnoughBloodForExtraction(Pawn pawn)
    {
        Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
        return firstHediffOfDef == null || firstHediffOfDef.Severity < MIN_BLOODLOSS_FOR_FAILURE;
    }
}