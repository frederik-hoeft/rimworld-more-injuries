using MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

[XmlBindable]
public partial class HediffCompHandler_SecondaryCondition_Tick : HediffCompHandler_SecondaryCondition, IHediffComp_SecondaryCondition_TickHandler
{
    [XmlBinding<int>("tickInterval", defaultValue: GenTicks.TickRareInterval)]
    public partial int TickInterval { get; }

    public virtual void Tick(HediffComp_SecondaryCondition comp)
    {
        if (!comp.Pawn.IsHashIntervalTick(TickInterval))
        {
            return;
        }
        HediffMakerDef hediffMakerDef = HediffMakerProps.GetHediffMakerDef(comp, handler: this);
        if (!ShouldSkip(comp, hediffMakerDef.HediffDef))
        {
            Evaluate(comp, hediffMakerDef);
        }
    }
}
