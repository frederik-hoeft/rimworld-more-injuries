using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

[XmlBindable]
public partial class HediffCompProperties_Choking : HediffCompProperties<HediffComp_Choking>
{
    [XmlBinding("tickInterval", NullableBackingField = true)]
    public partial int TickInterval { get; }

    [XmlBinding<int>("soundBackoffInterval", defaultValue: GenTicks.TickLongInterval)]
    public partial int SoundBackoffInterval { get; }

    [XmlBinding<float>("soundTriggerChance", defaultValue: 0.25f)]
    public partial float SoundTriggerChance { get; }

    [XmlBinding<float>("soundRandomizationChance", defaultValue: 0.25f)]
    public partial float SoundRandomizationChance { get; }
}
