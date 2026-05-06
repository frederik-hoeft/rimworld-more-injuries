using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Linked;

[XmlBindable]
public partial class HediffCompProperties_LinkedSeverity : HediffCompProperties
{
    public HediffCompProperties_LinkedSeverity() => compClass = typeof(HediffComp_LinkedSeverity);

    [XmlBinding<int>("tickInterval", defaultValue: GenTicks.TickRareInterval)]
    public partial int TickInterval { get; }

    [XmlBinding<float>("removeAtSeverity", defaultValue: 0f)]
    public partial float RemoveAtSeverity { get; }
}
