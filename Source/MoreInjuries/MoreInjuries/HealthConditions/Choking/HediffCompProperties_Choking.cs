using Verse;

namespace MoreInjuries.HealthConditions.Choking;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = "Members initialized via XML defs")]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffCompProperties_Choking : HediffCompProperties
{
    // don't rename this field. XML defs depend on this name
    private readonly int chokingIntervalTicks = default!;

    public HediffCompProperties_Choking() => compClass = typeof(HediffComp_Choking);

    public int ChokingIntervalTicks => chokingIntervalTicks;
}