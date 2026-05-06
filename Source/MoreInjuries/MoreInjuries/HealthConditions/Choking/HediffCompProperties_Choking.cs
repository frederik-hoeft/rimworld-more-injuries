using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

[XmlBindable]
public partial class HediffCompProperties_Choking : HediffCompProperties
{
    public HediffCompProperties_Choking() => compClass = typeof(HediffComp_Choking);

    [XmlBinding("chokingIntervalTicks", Validate = nameof(ValidateChokingIntervalTicks))]
    public partial int ChokingIntervalTicks { get; }

    private static bool ValidateChokingIntervalTicks(int ticks) => ticks > 0;
}
