using HarmonyLib;
using MoreInjuries.HealthConditions;
using Verse;

namespace MoreInjuries.Patches;

// private void DamageWorker_AddInjury::ApplyDamageToPart(DamageInfo dinfo, Pawn pawn, DamageWorker.DamageResult result)
[HarmonyPatch(typeof(DamageWorker_AddInjury), "ApplyDamageToPart", typeof(DamageInfo), typeof(Pawn), typeof(DamageWorker.DamageResult))]
public static class Patch_DamageWorker_AddInjury_ApplyDamageToPart
{
    internal static void Postfix(DamageInfo dinfo, Pawn pawn, DamageWorker.DamageResult result)
    {
        // only apply to non-null map to prevent conflicts with pawn generation
        if (pawn is { Map: not null } compHolder && compHolder.TryGetComp(out MoreInjuryComp comp))
        {
            comp.ApplyDamageToPart(in dinfo, pawn, result);
        }
    }
}