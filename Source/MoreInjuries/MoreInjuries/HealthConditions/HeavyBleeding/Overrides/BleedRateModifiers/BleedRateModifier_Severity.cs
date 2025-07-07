using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class BleedRateModifier_Severity : BleedRateModifier
{
    // don't rename this field. XML defs depend on this name
    private readonly float offset = 0f;

    public override float GetModifierFor(Hediff hediff, HediffWithComps bleedingHediff) => hediff.Severity + offset;
}