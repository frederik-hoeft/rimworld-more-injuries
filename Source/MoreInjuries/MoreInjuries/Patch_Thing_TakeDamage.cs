using HarmonyLib;
using MoreInjuries.HealthConditions;
using Verse;

namespace MoreInjuries;

[HarmonyPatch(typeof(Thing), nameof(Thing.TakeDamage))]
public static class Patch_Thing_TakeDamage
{
    internal static void Postfix(Thing __instance, DamageWorker.DamageResult __result)
    {
        // only apply to non-null map to prevent conflicts with pawn generation
        if (__instance is Pawn { Map: not null } compHolder && compHolder.TryGetComp(out InjuryComp comp) && comp.CallbackActive)
        {
            comp.PostDamageFull(__result);
        }
    }
}

