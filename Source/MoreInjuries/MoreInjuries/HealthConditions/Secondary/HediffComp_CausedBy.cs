using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

public sealed class HediffComp_CausedBy : HediffComp
{
    private List<string>? _causedBy;

    public override void CompExposeData() => Scribe_Collections.Look(ref _causedBy, "causedBy", saveDestroyedThings: false, LookMode.Value);

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

    public void ClearCauses() => _causedBy?.Clear();

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
            // fall through if there is invalid data. this cause tracking is just an optional feature
            return;
        }
        // lazy init of the list, and extract the base label if not provided
        _causedBy ??= [];
        causeLabelBase ??= GetCauseLabelBase(causeLabel);
        if (_causedBy.Count == 0)
        {
            // single cause, add it as is (may have a stage label or just be the base label)
            // this is the only path that allows a full label (base label + stage label) to be added
            _causedBy.Add(causeLabel);
            return;
        }
        if (_causedBy.Count == 1)
        {
            // we already have a single cause, we may need to:
            // - strip the stage label if it doesn't match the new cause
            // - merge the new cause with the existing one if they are the same stage, basically a no-op
            string originalCause = _causedBy[0];
            if (originalCause.Equals(causeLabel, StringComparison.Ordinal))
            {
                // the cause is exactly the same, no need to do anything
                return;
            }
            // attempt to get the base label of the original cause. This only succeeds if the downgrade is necessary
            // (fails if the original cause is already a base label)
            if (TryGetCauseLabelBase(originalCause, out string? originalCauseBase))
            {
                // replace the existing full label with the base label
                _causedBy[0] = originalCauseBase;
                if (originalCauseBase.Equals(causeLabelBase, StringComparison.Ordinal))
                {
                    // we downgraded the existing cause to its base label, and it matches the new cause's base label
                    // nothing more to do, the two causes are the same
                    return;
                }
            }
        }
        // if we reach here, we either:
        // - have multiple (base label) causes
        // - have a single cause that was downgraded to its base label, and it doesn't match the new cause's base label
        // - have a single base label cause, that may or may not match the new cause's base label
        if (_causedBy.IndexOf(causeLabelBase) != -1)
        {
            // already have this cause, no need to add it again
            return;
        }
        // add the new base label (which doesn't exist in the list yet)
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