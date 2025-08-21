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

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        int historySize = Mathf.CeilToInt(Properties.HistoryRetentionPeriodHours * GenDate.TicksPerHour / Properties.TickInterval);
        _severityChangeHistory = new SeverityHistory(historySize);
        Logger.LogDebug($"Initialized severity change history with size {historySize} for {parent.Label}.");
        base.CompPostPostAdd(dinfo);
        // tick immediately to sample the initial severity
        Tick(initializing: true);
    }

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Deep.Look(ref _severityChangeHistory, "severityHistory");
        Scribe_Values.Look(ref _previousSeverity, "previousSeverity");
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (parent.pawn.IsHashIntervalTick(Properties.TickInterval))
        {
            Tick(initializing: false);
        }
    }

    public void Tick(bool initializing)
    {
        DebugAssert.IsNotNull(_severityChangeHistory);
        SeverityHistoryEntry historyEntry = _severityChangeHistory.MoveNext();
        SampleLinkedHediffSeverities(historyEntry, parent.pawn, initializing);
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

    private void SampleLinkedHediffSeverities(SeverityHistoryEntry entry, Pawn pawn, bool initializing)
    {
        float totalSeverity = 0f;
        List<string>? hediffLabels = entry.HediffLabels;
        if (hediffLabels is not null)
        {
            hediffLabels.Clear();
        }
        else
        {
            hediffLabels = [];
        }
        foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
        {
            LinkedSeverityProperties_ModExtension? modExtension = hediff.def.GetModExtension<LinkedSeverityProperties_ModExtension>();
            if (modExtension is not null && modExtension.LinkedSeverityHandlers.TryGetValue(parent.def, out HediffCompHandler_LinkedSeverity? handler))
            {
                float effectiveSeverity = handler.Evaluate(hediff);
                if (effectiveSeverity > Mathf.Epsilon)
                {
                    totalSeverity += effectiveSeverity;
                    hediffLabels.Add(hediff.Label);
                }
            }
        }
        if (initializing)
        {
            // if we are initializing, it could be that there is already old/existing hypoxia damage
            // in that case, we don't want to immediately severity up to some unreasonably high value (since all previous history is empty / lost)
            // so we cap the total severity to the initialization threshold
            totalSeverity = Mathf.Min(totalSeverity, Properties.InitializationSeverityThreshold);
        }
        float severityChange = totalSeverity - _previousSeverity;
        _previousSeverity = totalSeverity;
        entry.SeverityChange = severityChange;
        entry.HediffLabels = hediffLabels;
    }

    private class SeverityHistoryEntry : IExposable
    {
        private float _severityChange;
        private List<string>? _hediffLabels;

        public float SeverityChange
        {
            get => _severityChange;
            set => _severityChange = value;
        }

        public List<string>? HediffLabels
        {
            get => _hediffLabels;
            set => _hediffLabels = value;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref _severityChange, "severityChange", 0f);
            Scribe_Collections.Look(ref _hediffLabels, "hediffLabels", LookMode.Value);
        }
    }

    private class SeverityHistory() : IExposable
    {
        /// <summary>
        /// Points to the current element in the history (the most recent entry) => pre-incremented when adding a new entry.
        /// </summary>
        private int _index;
        private SeverityHistoryEntry[] _entries = null!;

        public SeverityHistory(int size) : this()
        {
            Throw.ArgumentOutOfRangeException.IfNegativeOrZero(size);
            _entries = new SeverityHistoryEntry[size];
            for (int i = 0; i < _entries.Length; ++i)
            {
                _entries[i] = new SeverityHistoryEntry();
            }
        }

        private void IncrementIndex() => _index = (_index + 1) % _entries.Length;

        public SeverityHistoryEntry Current => _entries[_index];

        public SeverityHistoryEntry MoveNext()
        {
            IncrementIndex();
            return Current;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref _index, "index");
            List<SeverityHistoryEntry>? entries = null;
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                entries = [.. _entries];
            }
            Scribe_Collections.Look(ref entries, "entries", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Throw.InvalidOperationException.IfNull(this, entries);
                Throw.ArgumentOutOfRangeException.IfZero(entries.Count);
                _entries = new SeverityHistoryEntry[entries.Count];
                for (int i = 0; i < _entries.Length; ++i)
                {
                    _entries[i] = entries[i];
                }
            }
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
