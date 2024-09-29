using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Cannot use auto property for XML defs")]
public class ShockHediffCompProperties : HediffCompProperties
{
    // don't rename this field. XML defs depend on this name
    private readonly SimpleCurve _bleedSeverityCurve = default!;

    public ShockHediffCompProperties() => compClass = typeof(ShockHediffComp);

    public SimpleCurve BleedSeverityCurve => _bleedSeverityCurve;
}
