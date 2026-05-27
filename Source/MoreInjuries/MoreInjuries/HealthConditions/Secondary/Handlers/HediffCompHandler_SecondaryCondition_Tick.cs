using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

[XmlBindable]
public partial class HediffCompHandler_SecondaryCondition_Tick : HediffCompHandler_SecondaryCondition, IHediffComp_SecondaryCondition_TickHandler
{
    [XmlBinding<int>("tickInterval", defaultValue: GenTicks.TickRareInterval)]
    public partial int TickInterval { get; }
}
