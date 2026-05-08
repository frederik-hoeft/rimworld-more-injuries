using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlBindable]
public sealed partial class JobOutcomeDoer_HediffOffset : JobOutcomeDoer_HediffOffsetBase
{
    [XmlBinding("severityOffset")]
    public partial float SeverityOffset { get; }

    protected override float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device) => SeverityOffset;

    public override string ToString() => $"{base.ToString()} with fixed severity offset {SeverityOffset}";
}
