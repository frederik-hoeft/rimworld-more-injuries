using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlSerializable]
public sealed partial class JobOutcomeDoer_HediffOffset_Random : JobOutcomeDoer_HediffOffsetBase
{
    [XmlMember("severityOffsetRange")]
    public partial FloatRange SeverityOffsetRange { get; }

    protected override float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device) => SeverityOffsetRange.RandomInRange;

    public override string ToString() => $"{base.ToString()} with random severity offset range {SeverityOffsetRange}";
}
