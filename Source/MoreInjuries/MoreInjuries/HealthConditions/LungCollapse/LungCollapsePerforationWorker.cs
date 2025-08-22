using MoreInjuries.Debug;
using MoreInjuries.HealthConditions.HeavyBleeding.Overrides;
using RimWorld;
using System.Buffers;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse;

internal sealed class LungCollapsePerforationWorker(MoreInjuryComp parent) : LungCollapseWorkerBase(parent), IPostApplyDamageToPartHandler
{
    public override bool IsEnabled => base.IsEnabled && MoreInjuriesMod.Settings.LungCollapseChanceOnPerforatingDamage > 0f;

    public void ApplyDamageToPart(ref readonly DamageInfo dinfo, Pawn pawn, DamageWorker.DamageResult result)
    {
        DebugAssert.IsTrue(pawn == Pawn);
        float chance = MoreInjuriesMod.Settings.LungCollapseChanceOnPerforatingDamage;
        if (dinfo.HitPart is not { } lung 
            || lung.def != BodyPartDefOf.Lung 
            || result.hediffs is not { Count: > 0 } 
            || pawn.health.hediffSet.PartIsMissing(lung)
            // chance * (1 at 50% damage)
            || !Rand.Chance(chance * Mathf.Clamp01(2f * dinfo.Amount / lung.def.hitPoints)))
        {
            return;
        }
        Logger.LogDebug($"Running lung collapse calculations for {pawn.Name}");
        Hediff?[] causedBy = ArrayPool<Hediff?>.Shared.Rent(result.hediffs.Count);
        int i = 0;
        foreach (Hediff hediff in result.hediffs)
        {
            if (hediff is BetterInjury { Bleeding: true, Part: { } part } injury
                && part == lung
                && !injury.GetIsClosedInternalWound(forceRefresh: true))
            {
                // this lung has been perforated
                DebugAssert.IsTrue(i < causedBy.Length);
                causedBy[i++] = hediff;
            }
        }
        if (i == 0)
        {
            Logger.LogDebug($"Won't apply lung collapse to {pawn.Name} since it's either an internal or non-bleeding wound");
            return;
        }
        ReadOnlySpan<Hediff> causes = causedBy.AsSpan()[..i];
        CollapseLung(lung, causes);
        // clear array is critical to not keep hediffs alive longer than necessary
        ArrayPool<Hediff?>.Shared.Return(causedBy, clearArray: true);
    }
}
