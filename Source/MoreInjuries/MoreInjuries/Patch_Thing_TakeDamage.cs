using HarmonyLib;
using MoreInjuries.HealthConditions;
using Verse;

namespace MoreInjuries;

[HarmonyPatch(typeof(Thing), nameof(Thing.TakeDamage))]
public static class Patch_Thing_TakeDamage
{
    internal static bool IsActive { get; set; } = false;

    internal static void Postfix(Thing __instance, DamageWorker.DamageResult __result)
    {
        if (!IsActive)
        {
            return;
        }

        if (__instance is Pawn compHolder)
        {
            InjuryComp comp = compHolder.GetComp<InjuryComp>();
            comp.PostDamageFull(__result);
        }

        IsActive = false;
    }
}

