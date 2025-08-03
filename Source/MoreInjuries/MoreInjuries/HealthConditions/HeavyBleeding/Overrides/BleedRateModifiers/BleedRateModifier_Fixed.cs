using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class BleedRateModifier_Fixed : BleedRateModifier
{
    // do not rename these fields. XML defs depend on these names
    private readonly float factor = 1f;

    public float Factor => factor;

    public override float GetModifierFor(Hediff hediff, HediffWithComps bleedingHediff) => Factor;
}