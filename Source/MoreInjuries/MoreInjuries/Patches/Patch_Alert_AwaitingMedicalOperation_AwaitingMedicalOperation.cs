using HarmonyLib;
using MoreInjuries.Defs.WellKnown;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.Patches;

[HarmonyPatch(typeof(Alert_AwaitingMedicalOperation), "AwaitingMedicalOperation", MethodType.Getter)]
public static class Patch_Alert_AwaitingMedicalOperation_AwaitingMedicalOperation
{
    internal static void Postfix(ref List<Pawn> __result)
    {
        for (int i = __result.Count - 1; i >= 0; i--)
        {
            Pawn pawn = __result[i];
            if (pawn.IsPrisonerOfColony 
                && pawn.health.surgeryBills.Count == 1 
                && pawn.health.surgeryBills[0].recipe == KnownRecipeDefOf.ExtractWholeBloodBag 
                && pawn.guest.IsInteractionEnabled(KnownPrisonerInteractionModeDefOf.BloodBagFarm))
            {
                __result.RemoveAt(i);
            }
        }
    }
}
