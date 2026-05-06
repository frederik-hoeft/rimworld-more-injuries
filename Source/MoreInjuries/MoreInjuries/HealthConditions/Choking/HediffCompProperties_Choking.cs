using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

[XmlSerializable]
public partial class HediffCompProperties_Choking : HediffCompProperties
{
    public HediffCompProperties_Choking() => compClass = typeof(HediffComp_Choking);

    [XmlMember("chokingIntervalTicks", Validate = nameof(ValidateChokingIntervalTicks))]
    public partial int ChokingIntervalTicks { get; }

    private static bool ValidateChokingIntervalTicks(int ticks) => ticks > 0;
}
