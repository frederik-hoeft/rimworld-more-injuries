using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Linked;

[XmlSerializable]
public partial class HediffCompProperties_LinkedSeverity : HediffCompProperties
{
    public HediffCompProperties_LinkedSeverity() => compClass = typeof(HediffComp_LinkedSeverity);

    [XmlMember<int>("tickInterval", defaultValue: GenTicks.TickRareInterval)]
    public partial int TickInterval { get; }

    [XmlMember<float>("removeAtSeverity", defaultValue: 0f)]
    public partial float RemoveAtSeverity { get; }
}
