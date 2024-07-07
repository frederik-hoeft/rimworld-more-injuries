using HarmonyLib;
using Verse;

namespace MoreInjuries;

[HarmonyPatch(typeof(Thing), "TakeDamage")]
public static class Patch_Thing_TakeDamage
{
    public static bool Active = false;

    static void Postfix(Thing __instance, DamageWorker.DamageResult __result)
    {
        if (!Active)
            return;

        if (__instance is Pawn compHolder)
        {
            InjuriesComp comp = compHolder.GetComp<InjuriesComp>();
            comp.PostDamageFull(__result);
        }

        Active = false;
    }
}

