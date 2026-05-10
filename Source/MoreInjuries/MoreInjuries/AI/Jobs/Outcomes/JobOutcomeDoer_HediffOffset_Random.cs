using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlBindable]
public sealed partial class JobOutcomeDoer_HediffOffset_Random : JobOutcomeDoer_HediffOffsetBase
{
    [XmlBinding("severityOffsetRange")]
    public partial FloatRange SeverityOffsetRange { get; }

    protected override float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device) => SeverityOffsetRange.RandomInRange;

    public override string ToString() => $"{base.ToString()} with random severity offset range {SeverityOffsetRange}";
}
