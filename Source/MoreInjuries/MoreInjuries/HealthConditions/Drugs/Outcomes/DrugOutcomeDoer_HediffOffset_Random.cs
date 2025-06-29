using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Outcomes;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class DrugOutcomeDoer_HediffOffset_Random : DrugOutcomeDoer_HediffOffsetBase
{
    // don't rename this field. XML defs depend on this name
    private readonly FloatRange severityOffsetRange = default!;

    protected override float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device) => severityOffsetRange.RandomInRange;
}
