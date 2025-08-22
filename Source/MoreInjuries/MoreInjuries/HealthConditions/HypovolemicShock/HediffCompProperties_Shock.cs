using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = "Cannot use auto property for XML defs")]
public class HediffCompProperties_Shock : HediffCompProperties
{
    // don't rename this field. XML defs depend on this name
    private readonly SimpleCurve _bleedSeverityCurve = default!;

    public HediffCompProperties_Shock() => compClass = typeof(HediffComp_Shock);

    public SimpleCurve BleedSeverityCurve => _bleedSeverityCurve;
}