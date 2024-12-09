using HarmonyLib;
using MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.Patches;

[HarmonyPatch(typeof(ITab_Pawn_Visitor), nameof(ITab_Pawn_Visitor.NonExclusiveInteractionToggled))]
public static class Patch_ITab_Pawn_Visitor_NonExclusiveInteractionToggled
{
    internal static void Postfix(PrisonerInteractionModeDef mode, bool enabled)
    {
        Logger.LogDebug($"ITab_Pawn_Visitor.NonExclusiveInteractionToggled: {mode.defName} {enabled}");

        if (Find.Selector.SingleSelectedThing is not Pawn { Spawned: true } pawn)
        {
            return;
        }
        if (enabled && (mode == KnownPrisonerInteractionModeDefOf.BloodBagFarm || mode == PrisonerInteractionModeDefOf.HemogenFarm))
        {
            (PrisonerInteractionModeDef otherMode, RecipeDef otherRecipe) = mode == KnownPrisonerInteractionModeDefOf.BloodBagFarm
                ? (PrisonerInteractionModeDefOf.HemogenFarm, RecipeDefOf.ExtractHemogenPack)
                : (KnownPrisonerInteractionModeDefOf.BloodBagFarm, KnownRecipeDefOf.ExtractWholeBloodBag);

            if (pawn.guest.IsInteractionEnabled(otherMode))
            {
                Messages.Message($"{mode.label} and {otherMode.label} are mutually exclusive.", pawn, MessageTypeDefOf.RejectInput);
                pawn.guest.ToggleNonExclusiveInteraction(otherMode, false);
                pawn.BillStack?.Bills?.RemoveAll(b => b.recipe == otherRecipe);
            }
        }
        if (mode != KnownPrisonerInteractionModeDefOf.BloodBagFarm)
        {
            return;
        }
        Bill? bill = pawn.BillStack?.Bills?.Find(b => b.recipe == KnownRecipeDefOf.ExtractWholeBloodBag);
        if (enabled)
        {
            if (bill is not null || !Recipe_ExtractBloodBag.CanSafelyBeQueued(pawn))
            {
                return;
            }
            HealthCardUtility.CreateSurgeryBill(pawn, KnownRecipeDefOf.ExtractWholeBloodBag, part: null!);
        }
        else
        {
            if (bill is null)
            {
                return;
            }
            pawn.BillStack!.Bills.Remove(bill);
        }
    }
}
