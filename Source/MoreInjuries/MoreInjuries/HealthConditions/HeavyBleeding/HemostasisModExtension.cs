using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HemostasisModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly float coagulationMultiplier = default;

    // don't rename this field. XML defs depend on this name
    private readonly int disappearsAfterTicks = default;

    public float CoagulationMultiplier => coagulationMultiplier;

    public int DisappearsAfterTicks => disappearsAfterTicks;
}
