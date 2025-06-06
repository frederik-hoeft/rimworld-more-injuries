using System.Text;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

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