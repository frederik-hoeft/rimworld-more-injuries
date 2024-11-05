using MoreInjuries.Extensions;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Fractures;

public class Recipe_SplintFracture : Recipe_RemoveHediff
{
    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        if (billDoer is null)
        {
            Logger.Warning($"{nameof(Recipe_SplintFracture)} was called with a null {nameof(billDoer)}");
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
        if (pawn.health.hediffSet.TryGetFirstHediffMatchingPart(part, recipe.removesHediff, out Hediff? hediff))
        {
            JobDriver_UseSplint.SplintFracture(billDoer, pawn, hediff!, part, severityOffset: -0.05f);
        }
    }
}
