using Verse;

namespace MoreInjuries.HypovolemicShock;

public class ShockHediffCompProperties : HediffCompProperties
{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    // don't rename this field. XML defs depend on this name
    private readonly SimpleCurve _bleedSeverityCurve = default!;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

    public ShockHediffCompProperties() => compClass = typeof(ShockHediffComp);

    public SimpleCurve BleedSeverityCurve => _bleedSeverityCurve;
}
