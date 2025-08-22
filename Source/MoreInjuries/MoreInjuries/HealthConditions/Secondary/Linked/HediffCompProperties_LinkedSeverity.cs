using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Linked;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffCompProperties_LinkedSeverity : HediffCompProperties
{
    // don't rename this field. XML defs depend on this name
    private readonly int tickInterval = GenTicks.TickRareInterval;
    // don't rename this field. XML defs depend on this name
    private readonly float removeAtSeverity = 0f;

    public HediffCompProperties_LinkedSeverity() => compClass = typeof(HediffComp_LinkedSeverity);

    public int TickInterval => tickInterval;

    public float RemoveAtSeverity => removeAtSeverity;
}