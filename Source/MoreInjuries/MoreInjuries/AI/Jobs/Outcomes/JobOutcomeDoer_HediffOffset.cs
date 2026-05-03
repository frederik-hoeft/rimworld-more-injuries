using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlSerializable]
public sealed partial class JobOutcomeDoer_HediffOffset : JobOutcomeDoer_HediffOffsetBase
{
    [XmlMember("severityOffset")]
    public partial float SeverityOffset { get; }

    protected override float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device) => SeverityOffset;

    public override string ToString() => $"{base.ToString()} with fixed severity offset {SeverityOffset}";
}
