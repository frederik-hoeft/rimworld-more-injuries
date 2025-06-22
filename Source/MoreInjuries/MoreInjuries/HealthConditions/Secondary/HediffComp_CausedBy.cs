using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

public sealed class HediffComp_CausedBy : HediffComp
{
    private List<string>? _causedBy;

    public override void CompExposeData()
    {
        Scribe_Collections.Look(ref _causedBy, "causedBy", saveDestroyedThings: false, LookMode.Value);
    }

    public void CopyFrom(Hediff otherHediff)
    {
        if (otherHediff.TryGetComp(out HediffComp_CausedBy other) && other._causedBy is { Count: > 0 })
        {
            foreach (string cause in other._causedBy)
            {
                AddCause(cause);
            }
        }
    }

    public void AddCause(Hediff cause)
    {
        if (cause is null)
        {
            return;
        }
        AddCause(cause.Label, cause.LabelBase);
    }

    public void AddCause(string causeLabel, string? causeLabelBase = null)
    {
        if (string.IsNullOrEmpty(causeLabel))
        {
            return;
        }
        _causedBy ??= [];
        causeLabelBase ??= GetCauseLabelBase(causeLabel);
        if (_causedBy.Count == 0)
        {
            _causedBy.Add(causeLabel);
            return;
        }
        if (_causedBy.Count == 1)
        {
            string originalCause = _causedBy[0];
            if (originalCause.Equals(causeLabel, StringComparison.Ordinal))
            {
                return;
            }
            if (TryGetCauseLabelBase(originalCause, out string? originalCauseBase))
            {
                _causedBy[0] = originalCauseBase;
                if (originalCauseBase.Equals(causeLabelBase, StringComparison.Ordinal))
                {
                    return;
                }
            }
        }
        else if (_causedBy.IndexOf(causeLabelBase) != -1)
        {
            return;
        }
        _causedBy.Add(causeLabelBase);
    }

    private static string GetCauseLabelBase(string causeLabel)
    {
        if (TryGetCauseLabelBase(causeLabel, out string? causeLabelBase))
        {
            return causeLabelBase;
        }
        // if we can't find a base label, we return the original label
        return causeLabel;
    }

    private static bool TryGetCauseLabelBase(string causeLabel, [NotNullWhen(true)] out string? causeLabelBase)
    {
        if (causeLabel.LastIndexOf(" (") is int index && index != 0 && causeLabel[causeLabel.Length - 1] == ')')
        {
            causeLabelBase = causeLabel.Substring(0, index);
            return true;
        }
        causeLabelBase = null;
        return false;
    }

    public override string CompTipStringExtra => _causedBy is { Count: > 0 }
        ? $"\n{"Cause".Translate()}: {string.Join(", ", _causedBy.Select(static cause => cause.Colorize(ColoredText.ThreatColor)))}".Colorize(ColoredText.SubtleGrayColor)
        : base.CompTipStringExtra;
}