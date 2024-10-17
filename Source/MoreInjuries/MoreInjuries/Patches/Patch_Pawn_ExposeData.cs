using HarmonyLib;
using MoreInjuries.HealthConditions;
using MoreInjuries.Initialization;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace MoreInjuries.Patches;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.ExposeData))]
public static class Patch_Pawn_ExposeData
{
    private static AccessTools.FieldRef<Pawn_HealthTracker, PawnHealthState>? s_pawnHealthState;

    public static AccessTools.FieldRef<Pawn_HealthTracker, PawnHealthState> PawnHealthStateRef => s_pawnHealthState
        ??= AccessTools.FieldRefAccess<Pawn_HealthTracker, PawnHealthState>("healthState");

    internal static void Postfix(Pawn __instance)
    {
        if (Scribe.mode is LoadSaveMode.PostLoadInit && __instance is Pawn compHolder && compHolder.TryGetComp(out MoreInjuryComp comp) && comp.FailedLoading)
        {
            // prevent double-patching
            comp.FailedLoading = false;
            // run compatibility fixes to prevent pawn from dying due to inconsistent hediffs
            Logger.Log($"Running compatibility patch to remove all possible hediff conflicts from {compHolder.Label}");
            List<Hediff> hediffs = compHolder.health.hediffSet.hediffs;
            bool requiredPatching = false;
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                Hediff hediff = hediffs[i];
                if (hediff is Hediff_Injury or Hediff_MissingPart)
                {
                    Logger.Log($"Removing conflicting hediff {hediff} from pawn {compHolder.Label} because the target body part may be incorrect");
                    hediffs.Remove(hediff);
                    requiredPatching = true;
                }
            }
            // fix any misplaced bionics
            FixMisplacedBionicsModExtension.FixPawn(compHolder);
            if (compHolder.Dead)
            {
                Logger.Warning($"Pawn {compHolder.Label} died due incorrect hediff placement after load failure. Fully resetting pawn ...");
                ref PawnHealthState healthState = ref PawnHealthStateRef.Invoke(compHolder.health);
                healthState = PawnHealthState.Mobile;
                compHolder.health.capacities.Clear();
                compHolder.health.summaryHealth.Notify_HealthChanged();
                compHolder.health.surgeryBills.Clear();
                compHolder.health.immunity = new ImmunityHandler(compHolder);
            }
            if (requiredPatching)
            {
                Logger.Warning($"Compatibility patch removed some hediff(s) from pawn {compHolder.Label} due to load failures");
            }
        }
    }
}
