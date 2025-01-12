using MoreInjuries.Localization;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions;

public class Recipe_RemoveHediff_WithReusableIngredients : Recipe_RemoveHediff
{
    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        if (billDoer is not null)
        {
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
        }
        Hediff? hediff = pawn.health.hediffSet.hediffs.Find(x => x.def == recipe.removesHediff && x.Part == part && x.Visible);
        if (hediff is null)
        {
            return;
        }

        pawn.health.RemoveHediff(hediff);

        foreach (Thing ingredient in ingredients)
        {
            if (ingredient.def.GetModExtension<ReusabilityProps_ModExtension>() is { DestroyChance: float destroyChance })
            {
                if (Rand.Chance(destroyChance))
                {
                    Messages.Message("MI_Message_SurgeryIngredientDestroyed".Translate(billDoer.Named(Named.Params.DOCTOR), ingredient.Named(Named.Params.THING)), billDoer, MessageTypeDefOf.NegativeEvent);
                }
                else
                {
                    Thing copy = ThingMaker.MakeThing(ingredient.def);
                    if (ingredient.def.useHitPoints && ingredient.HitPoints > 0)
                    {
                        copy.HitPoints = ingredient.HitPoints;
                    }
                    if (!GenPlace.TryPlaceThing(copy, pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near))
                    {
                        Logger.Error($"Could not drop blood bag near {pawn.PositionHeld}");
                    }
                }
            }
        }
    }
}