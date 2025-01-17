using MoreInjuries.Localization;
using MoreInjuries.Things;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions;

public class Recipe_RemoveHediff_WithReusableIngredients : Recipe_RemoveHediff
{
    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        if (billDoer is null)
        {
            Logger.Error($"Recipe_RemoveHediff_WithReusableIngredients: {nameof(billDoer)} cannot be null");
            return;
        }
        if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
        {
            return;
        }

        TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
        if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(billDoer))
        {
            Messages.Message(recipe.successfullyRemovedHediffMessage.NullOrEmpty() 
                ? "MessageSuccessfullyRemovedHediff".Translate(billDoer.LabelShort, pawn.LabelShort, recipe.removesHediff.label.Named("HEDIFF"), billDoer.Named("SURGEON"), pawn.Named("PATIENT")) 
                : recipe.successfullyRemovedHediffMessage.Formatted(billDoer.LabelShort, pawn.LabelShort), pawn, MessageTypeDefOf.PositiveEvent);
        }
        Hediff? hediff = pawn.health.hediffSet.hediffs.Find(x => x.def == recipe.removesHediff && x.Part == part && x.Visible);
        if (hediff is null)
        {
            return;
        }

        pawn.health.RemoveHediff(hediff);

        foreach (Thing ingredient in ingredients)
        {
            ReusabilityUtility.TryReuseSurgeryIngredient(ingredient, billDoer, pawn);
        }
    }
}
