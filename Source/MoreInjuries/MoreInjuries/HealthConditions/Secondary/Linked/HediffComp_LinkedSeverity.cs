using MoreInjuries.Caching;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Linked;

public sealed class HediffComp_LinkedSeverity : HediffComp
{
    private readonly WeakPawnDataCache<IReadOnlyList<LinkedSeverityData>> _severityDataCache;

    public HediffComp_LinkedSeverity()
    {
        _severityDataCache = new WeakPawnDataCache<IReadOnlyList<LinkedSeverityData>>(dataProvider: GetLinkedHediffSeverityData);
    }

    private HediffCompProperties_LinkedSeverity Properties => (HediffCompProperties_LinkedSeverity)props;

    private IReadOnlyList<LinkedSeverityData> GetLinkedHediffSeverityData(Pawn pawn)
    {
        if (pawn.health.hediffSet.hediffs.Count == 0)
        {
            return Array.Empty<LinkedSeverityData>();
        }
        List<LinkedSeverityData> linkedSeverities = [];
        foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
        {
            LinkedSeverityProperties_ModExtension? modExtension = hediff.def.GetModExtension<LinkedSeverityProperties_ModExtension>();
            if (modExtension is not null && modExtension.LinkedSeverityHandlers.TryGetValue(parent.def, out HediffCompHandler_LinkedSeverity? handler))
            {
                float effectiveSeverity = handler.Evaluate(hediff);
                if (effectiveSeverity > 0f)
                {
                    linkedSeverities.Add(new LinkedSeverityData(hediff.LabelCap, effectiveSeverity));
                }
            }
        }
        return linkedSeverities;
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (!parent.pawn.IsHashIntervalTick(Properties.TickInterval))
        {
            return;
        }
        IReadOnlyList<LinkedSeverityData> linkedSeverityData = _severityDataCache.GetData(parent.pawn, forceRefresh: true);
        if (linkedSeverityData.Count == 0)
        {
            return;
        }
        float totalSeverity = Properties.RemoveAtSeverity;
        if (parent.TryGetComp(out HediffComp_CausedBy? causedBy))
        {
            causedBy!.ClearCauses();
            foreach (LinkedSeverityData data in linkedSeverityData)
            {
                totalSeverity += data.EffectiveSeverity;
                causedBy.AddCause(data.HediffLabel);
            }
        }
        else
        {
            foreach (LinkedSeverityData data in linkedSeverityData)
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