using System.Text;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

// TODO: support multiple causes (e.g., when multiple hypoxia hediffs are compressed into one hediff)
// in this case it should show "Caused by: <cause1>, <cause2>, ..." in the tooltip
// as soon as there are multiple causes, stage labels should be stripped. E.g., no "Caused by: <cause1> (Stage 1), <cause1> (Stage 2)"
// TODO: also, maybe distinguish between different types of hypoxia?
public class HediffComp_CausedBy : HediffComp
{
    private string? _causedBy;

    public string? CausedBy 
    { 
        get => _causedBy; 
        set => _causedBy = value; 
    }

    public override void CompExposeData()
    {
        Scribe_Values.Look(ref _causedBy, "causedBy", defaultValue: null);
    }

    public override string CompTipStringExtra => !string.IsNullOrEmpty(_causedBy)
        ? $"\n{"Cause".Translate()}: {_causedBy}".Colorize(ColoredText.SubtleGrayColor)
        : base.CompTipStringExtra;
}