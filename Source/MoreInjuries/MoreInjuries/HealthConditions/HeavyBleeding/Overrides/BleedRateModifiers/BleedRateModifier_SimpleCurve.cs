using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class BleedRateModifier_SimpleCurve : BleedRateModifier
{
    // do not rename these fields. XML defs depend on these names
    private readonly SimpleCurve severityCurve = default!;

    public SimpleCurve SeverityCurve => severityCurve;

    public override float GetModifierFor(Hediff hediff, HediffWithComps bleedingHediff) => SeverityCurve.Evaluate(hediff.Severity);
}