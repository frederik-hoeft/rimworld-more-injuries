using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class JobOutcomeDoer_HediffOffset : JobOutcomeDoer_HediffOffsetBase
{
    // don't rename this field. XML defs depend on this name
    private readonly float severityOffset = default;

    protected override float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device) => severityOffset;
}