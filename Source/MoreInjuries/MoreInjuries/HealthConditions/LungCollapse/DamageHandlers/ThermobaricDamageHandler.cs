using MoreInjuries.Defs.WellKnown;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse.DamageHandlers;

internal sealed class ThermobaricDamageHandler : ILungCollapseHandler
{
    public bool EvaluateDamageChances(LungCollapseWorker worker, ref readonly DamageInfo dinfo, ref LungCollapseEvaluationContext context, ref List<Hediff>? causedBy)
    {
        if (dinfo.Def != DamageDefOf.Bomb && dinfo.Def != KnownDamageDefOf.CE_Thermobaric)
        {
            return false;
        }
        float chance = MoreInjuriesMod.Settings.LungCollapseChanceOnThermobaricDamage;
        if (chance < Mathf.Epsilon)
        {
            return false;
        }
        for (int i = 0; i < context.ChancesPerLung.Length; ++i)
        {
            context.ChancesPerLung[i] += chance;
        }
        Logger.LogDebug("true");
        return true;
    }
}
