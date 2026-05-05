using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Acidosis;

[XmlSerializable]
public sealed partial class HediffCompProperties_Acidosis : HediffCompProperties
{
    public HediffCompProperties_Acidosis() => compClass = typeof(HediffComp_Acidosis);

    [XmlMember<float>("historyRetentionPeriodHours", defaultValue: 1f)]
    public partial float HistoryRetentionPeriodHours { get; }

    [XmlMember<int>("tickInterval", defaultValue: 250)]
    public partial int TickInterval { get; }

    [XmlMember<float>("recoveryPerDay", defaultValue: 8)]
    public partial float RecoveryPerDay { get; }

    [XmlMember<float>("initializationSeverityThreshold", defaultValue: 5f)]
    public partial float InitializationSeverityThreshold { get; }
}
