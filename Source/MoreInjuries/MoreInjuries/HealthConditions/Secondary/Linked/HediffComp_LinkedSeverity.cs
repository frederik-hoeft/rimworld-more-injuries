using MoreInjuries.Caching;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Linked;

public sealed class HediffComp_LinkedSeverity : HediffComp
{
    private static readonly ObjectPool<Poolable<List<LinkedSeverityData>>> s_pooledSeverityDataLists = new
    (
        maxCapacity: 16,
        factory: static pool => new Poolable<List<LinkedSeverityData>>
        (
            pool,
            canPool: static value => value.Capacity <= 8,
            factory: static () => [],
            reset: static value => value.Clear()
        )
    );

    private HediffCompProperties_LinkedSeverity Properties => (HediffCompProperties_LinkedSeverity)props;

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        base.CompPostPostAdd(dinfo);
        // we need to tick this hediff immediately to set the initial severity
        Tick();
    }

    private Poolable<List<LinkedSeverityData>>? GetLinkedHediffSeverityData(Pawn pawn)
    {
        Poolable<List<LinkedSeverityData>> result = s_pooledSeverityDataLists.Rent();
        result.Initialize();
        List<LinkedSeverityData> linkedSeverities = result.Value;
        foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
        {
            LinkedSeverityProperties_ModExtension? modExtension = hediff.def.GetModExtension<LinkedSeverityProperties_ModExtension>();
            if (modExtension is not null && modExtension.LinkedSeverityHandlers.TryGetValue(parent.def, out HediffCompHandler_LinkedSeverity? handler))
            {
                float effectiveSeverity = handler.Evaluate(hediff);
                if (effectiveSeverity > 0f)
                {
                    linkedSeverities.Add(new LinkedSeverityData(hediff.Label, effectiveSeverity));
                }
            }
        }
        return result;
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (parent.pawn.IsHashIntervalTick(Properties.TickInterval))
        {
            Tick();
        }
    }

    public void Tick()
    {
        using Poolable<List<LinkedSeverityData>>? linkedSeverityData = GetLinkedHediffSeverityData(parent.pawn);
        if (linkedSeverityData is not { Value: { Count: > 0 } linkedSeverities })
        {
            return;
        }
        float totalSeverity = Properties.RemoveAtSeverity;
        if (parent.TryGetComp(out HediffComp_CausedBy? causedBy))
        {
            causedBy!.ClearCauses();
            foreach (LinkedSeverityData data in linkedSeverities)
            {
                totalSeverity += data.EffectiveSeverity;
                causedBy.AddCause(data.HediffLabel);
            }
        }
        else
        {
            foreach (LinkedSeverityData data in linkedSeverities)
            {
                totalSeverity += data.EffectiveSeverity;
            }
        }
        totalSeverity = Mathf.Max(totalSeverity, Mathf.Epsilon);
        if (Mathf.Approximately(totalSeverity, Properties.RemoveAtSeverity))
        {
            // remove the hediff
            parent.pawn.health.RemoveHediff(parent);
            return;
        }
        // set the severity to the total of all linked severities
        if (totalSeverity != parent.Severity)
        {
            parent.Severity = totalSeverity;
        }
    }

    private readonly record struct LinkedSeverityData(string HediffLabel, float EffectiveSeverity);
}