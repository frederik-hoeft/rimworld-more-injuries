using MoreInjuries.Debug;
using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Linked;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Acidosis;

public sealed class HediffComp_Acidosis : HediffComp
{
    private SeverityHistory? _severityChangeHistory;
    private float _previousSeverity;

    private HediffCompProperties_Acidosis Properties => (HediffCompProperties_Acidosis)props;

    // TODO: once acidosis expires, is removed, and re-created by hypoxia, the history is "forgotten", and already handled/old hypoxia damage is incorrectly "rediscovered".
    // => we may need to keep acidosis around for a while (in a hidden state) after it expires until all hypoxia damage is healed.
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        int historySize = Mathf.CeilToInt(Properties.HistoryRetentionPeriodHours * GenDate.TicksPerHour / Properties.TickInterval);
        _severityChangeHistory = new SeverityHistory(historySize);
        Logger.LogDebug($"Initialized severity change history with size {historySize} for {parent.Label}.");
        base.CompPostPostAdd(dinfo);
        // tick immediately to sample the initial severity
        Tick();
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
        DebugAssert.IsNotNull(_severityChangeHistory);
        SeverityHistoryEntry severityHistoryEntry = SampleLinkedHediffSeverities(parent.pawn);
        _severityChangeHistory.Add(severityHistoryEntry);
        HediffComp_CausedBy? causedBy = parent.GetComp<HediffComp_CausedBy>();
        float averageSeverityChange = _severityChangeHistory.CalculateAverageChange(causedBy);
        float severity = parent.Severity;
        float newSeverity;
        if (averageSeverityChange > Mathf.Epsilon)
        {
            // if the average severity change is positive, we increase the severity
            newSeverity = severity + averageSeverityChange;
            parent.Severity = newSeverity;
        }
        else
        {
            // otherwise, we decrease with the recovery rate
            newSeverity = severity - (Properties.RecoveryPerDay * Properties.TickInterval / GenDate.TicksPerDay);
            if (newSeverity < Mathf.Epsilon)
            {
                // remove the hediff
                parent.pawn.health.RemoveHediff(parent);
            }
            else
            {
                parent.Severity = newSeverity;
            }
        }
        Logger.LogDebug($"Severity of {parent.Label} changed: {severity:F2} -> {newSeverity:F2}");
    }

    private SeverityHistoryEntry SampleLinkedHediffSeverities(Pawn pawn)
    {
        float totalSeverity = 0f;
        List<string>? hediffLabels = null;
        foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
        {
            LinkedSeverityProperties_ModExtension? modExtension = hediff.def.GetModExtension<LinkedSeverityProperties_ModExtension>();
            if (modExtension is not null && modExtension.LinkedSeverityHandlers.TryGetValue(parent.def, out HediffCompHandler_LinkedSeverity? handler))
            {
                float effectiveSeverity = handler.Evaluate(hediff);
                if (effectiveSeverity > Mathf.Epsilon)
                {
                    totalSeverity += effectiveSeverity;
                    hediffLabels ??= [];
                    hediffLabels.Add(hediff.Label);
                }
            }
        }
        float severityChange = totalSeverity - _previousSeverity;
        _previousSeverity = totalSeverity;
        SeverityHistoryEntry entry = new(severityChange, hediffLabels);
        return entry;
    }

    private readonly record struct SeverityHistoryEntry(float SeverityChange, List<string>? HediffLabels);

    private class SeverityHistory
    {
        private readonly SeverityHistoryEntry[] _entries;

        public SeverityHistory(int size)
        {
            Throw.ArgumentOutOfRangeException.IfNegativeOrZero(size);
            _entries = new SeverityHistoryEntry[size];
        }

        public void Add(SeverityHistoryEntry entry)
        {
            for (int i = 0; i < _entries.Length - 1; i++)
            {
                _entries[i] = _entries[i + 1];
            }
            _entries[^1] = entry;
        }

        public float CalculateAverageChange(HediffComp_CausedBy? causedBy)
        {
            float totalChange = 0f;
            foreach (SeverityHistoryEntry data in _entries)
            {
                totalChange += data.SeverityChange;
                if (data.HediffLabels is not null && causedBy is not null)
                {
                    foreach (string label in data.HediffLabels)
                    {
                        causedBy.AddCause(label);
                    }
                }
            }
            return totalChange / _entries.Length;
        }
    }
}
