using MoreInjuries.HealthConditions.HeavyBleeding.Overrides;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse.DamageHandlers;

// TODO: simplify now that we already get the hediff causing the injury
internal sealed class PerforatingDamageHandler : ILungCollapseHandler
{
    public bool EvaluateDamageChances(LungCollapseWorker worker, ref readonly DamageInfo dinfo, ref LungCollapseEvaluationContext context, ref List<Hediff>? causedBy)
    {
        float chance = MoreInjuriesMod.Settings.LungCollapseChanceOnPerforatingDamage;
        if (chance < Mathf.Epsilon || dinfo.HitPart is not { } lung || lung.def != BodyPartDefOf.Lung)
        {
            if (dinfo.HitPart is not null)
            {
                Logger.LogDebug($"no lung damage: hit {dinfo.HitPart.Label} instead");
            }
            return false;
        }
        Pawn patient = worker.Pawn;
        // populate lungs if necessary
        Span<BodyPartRecord?> lungs = context.Lungs;
        // prefer direct access over bounds check through span
        if (context.Lung0 == null)
        {
            int i = 0;
            foreach (BodyPartRecord bodyPart in patient.health.hediffSet.GetNotMissingParts())
            {
                if (i >= lungs.Length)
                {
                    break;
                }
                if (bodyPart.def == BodyPartDefOf.Lung)
                {
                    lungs[i++] = bodyPart;
                }
            }
        }
        int lungIndex = -1;
        for (int i = 0; i < lungs.Length; i++)
        {
            if (lungs[i] == lung)
            {
                lungIndex = i;
                break;
            }
        }
        if (lungIndex == -1)
        {
            Logger.ConfigError($"Did no find correct lung to apply lung collapse to");
            return false;
        }
        foreach (Hediff hediff in patient.health.hediffSet.hediffs)
        {
            if (hediff is BetterInjury { Bleeding: true, Part: { } part } injury 
                && part == lung
                && !injury.GetIsClosedInternalWound(forceRefresh: true))
            {
                // this lung has been perforated
                context.ChancesPerLung[lungIndex] += chance;
                causedBy ??= [];
                causedBy.Add(hediff);
                Logger.LogVerbose($"perforated lung with total chance {context.ChancesPerLung[lungIndex]}");
                return true;
            }
        }
        return false;
    }
}
