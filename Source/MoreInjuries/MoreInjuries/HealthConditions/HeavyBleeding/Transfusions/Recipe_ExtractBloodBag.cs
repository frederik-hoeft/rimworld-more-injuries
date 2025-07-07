using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Localization;
using RimWorld;
using System.Collections.Generic;
using Verse;

using static MoreInjuries.HealthConditions.HeavyBleeding.BloodLossConstants;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

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
        
        return pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness) > 0.45f && pawn.health.CanBleed && base.AvailableOnNow(thing, part);
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

        Messages.Message("MI_DonateBloodFailed_MissingBloodVolume_Message".Translate(medPawn.Named(Named.Params.PATIENT)), medPawn, MessageTypeDefOf.NeutralEvent, false);
    }

    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        if (!PawnHasEnoughBloodForExtraction(pawn))
        {
            Messages.Message("MI_DonateBloodFailed_MissingBloodVolume_Message".Translate(pawn.Named(Named.Params.PATIENT)), pawn, MessageTypeDefOf.NeutralEvent);
            return;
        }
        Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, pawn);
        hediff.Severity = BLOOD_LOSS_SEVERITY;
        pawn.health.AddHediff(hediff);
        OnSurgerySuccess(pawn, part, billDoer, ingredients, bill);
        if (!IsViolationOnPawn(pawn, part, Faction.OfPlayer))
        {
            if (pawn.needs.mood is not null)
            {
                Thought_Memory thought = (Thought_Memory)ThoughtMaker.MakeThought(KnownThoughtDefOf.DonatedBlood);
                pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
            }
            return;
        }

        ReportViolation(pawn, billDoer, pawn.HomeFaction, -1, KnownHistoryEventDefOf.ExtractedWholeBloodBag);
    }

    protected override void OnSurgerySuccess(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        if (GenPlace.TryPlaceThing(ThingMaker.MakeThing(JobDriver_UseBloodBag.JobDeviceDef), pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near))
        {
            return;
        }
        Logger.Error($"Could not drop blood bag near {pawn.PositionHeld}");
    }

    private static bool PawnHasEnoughBloodForExtraction(Pawn pawn)
    {
        Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
        return firstHediffOfDef == null || firstHediffOfDef.Severity + BLOOD_LOSS_SEVERITY < MIN_BLOODLOSS_FOR_FAILURE;
    }

    public static bool CanSafelyBeQueued(Pawn pawn) => KnownRecipeDefOf.ExtractWholeBloodBag.Worker is Recipe_Surgery worker
        && worker.AvailableOnNow(pawn)
        && worker.CompletableEver(pawn);
}