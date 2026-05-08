using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Acidosis;

[XmlBindable]
public sealed partial class HediffCompProperties_Acidosis : HediffCompProperties
{
    public HediffCompProperties_Acidosis() => compClass = typeof(HediffComp_Acidosis);

    [XmlBinding<float>("historyRetentionPeriodHours", defaultValue: 1f)]
    public partial float HistoryRetentionPeriodHours { get; }

    [XmlBinding<int>("tickInterval", defaultValue: 250)]
    public partial int TickInterval { get; }

    [XmlBinding<float>("recoveryPerDay", defaultValue: 8)]
    public partial float RecoveryPerDay { get; }

    [XmlBinding<float>("initializationSeverityThreshold", defaultValue: 5f)]
    public partial float InitializationSeverityThreshold { get; }
}
