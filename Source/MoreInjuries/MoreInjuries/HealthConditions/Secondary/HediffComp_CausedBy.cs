using System.Collections.Generic;
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

    public void AddCause(Hediff cause)
    {
        if (cause is null)
        {
            return;
        }
        _causedBy ??= [];
        if (_causedBy.Count == 0)
        {
            _causedBy.Add(cause.Label);
            return;
        }
        if (_causedBy.Count == 1)
        {
            string originalCause = _causedBy[0];
            if (originalCause.Equals(cause.Label, StringComparison.Ordinal))
            {
                return;
            }
            if (originalCause.LastIndexOf(" (") is int index && index != 0 && originalCause[originalCause.Length - 1] == ')')
            {
                originalCause = _causedBy[0] = originalCause.Substring(0, index);
                if (originalCause.Equals(cause.LabelBase, StringComparison.Ordinal))
                {
                    return;
                }
            }
        }
        else if (_causedBy.IndexOf(cause.LabelBase) != -1)
        {
            return;
        }
        _causedBy.Add(cause.LabelBase);
    }

    public override string CompTipStringExtra => _causedBy is { Count: > 0 }
        ? $"\n{"Cause".Translate()}: {string.Join(", ", _causedBy.Select(static cause => cause.Colorize(ColoredText.ThreatColor)))}".Colorize(ColoredText.SubtleGrayColor)
        : base.CompTipStringExtra;
}