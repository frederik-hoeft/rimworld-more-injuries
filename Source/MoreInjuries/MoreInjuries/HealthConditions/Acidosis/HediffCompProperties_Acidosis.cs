using Verse;

namespace MoreInjuries.HealthConditions.Acidosis;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class HediffCompProperties_Acidosis : HediffCompProperties
{
    // don't rename this field. XML defs depend on this name
    private readonly float historyRetentionPeriodHours = default;
    // don't rename this field. XML defs depend on this name
    private readonly int tickInterval = default;
    // don't rename this field. XML defs depend on this name
    private readonly float recoveryPerDay = default;
    // don't rename this field. XML defs depend on this name
    private readonly float initializationSeverityThreshold = default;

    public HediffCompProperties_Acidosis() => compClass = typeof(HediffComp_Acidosis);

    public float HistoryRetentionPeriodHours => historyRetentionPeriodHours;

    public int TickInterval => tickInterval;

    public float RecoveryPerDay => recoveryPerDay;

    public float InitializationSeverityThreshold => initializationSeverityThreshold;
}
