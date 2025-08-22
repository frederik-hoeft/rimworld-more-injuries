using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse;

internal sealed class LungCollapseThermobaricWorker(MoreInjuryComp parent) : LungCollapseWorkerBase(parent), IPostPostApplyDamageHandler
{
    public override bool IsEnabled => base.IsEnabled && MoreInjuriesMod.Settings.LungCollapseChanceOnThermobaricDamage > 0f;

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        if (dinfo.Def != DamageDefOf.Bomb && dinfo.Def != KnownDamageDefOf.CE_Thermobaric)
        {
            return;
        }
        Pawn patient = Pawn;
        float chance = MoreInjuriesMod.Settings.LungCollapseChanceOnThermobaricDamage;
        // small explosions won't do as much thermobaric damage
        float damageChance = Mathf.Clamp01(dinfo.Amount / 20f);
        bool chanceLung1 = Rand.Chance(chance * damageChance);
        bool chanceLung2 = Rand.Chance(chance * damageChance);
        if (!chanceLung1 && !chanceLung2)
        {
            // early exit if both chances fail
            return;
        }
        ReadOnlySpan<bool> chances = [chanceLung1, chanceLung2];
        int i = 0;
        ReadOnlySpan<string> causes = dinfo.WeaponLinkedHediff is { label: string cause } ? [cause] : [];
        foreach (BodyPartRecord lung in patient.health.hediffSet.GetNonMissingPartsOfType(BodyPartDefOf.Lung))
        {
            if (i < chances.Length && chances[i++])
            {
                CollapseLung(lung, causes);
            }
            else if (i > chances.Length)
            {
                Logger.Warning($"{patient.Name} has more than 2 lungs? WTF is wrong with them??");
                if (Rand.Chance(chance))
                {
                    CollapseLung(lung, causes);
                }
            }
        }
    }
}