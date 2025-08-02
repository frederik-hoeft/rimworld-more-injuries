using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffCompHandler_SecondaryCondition_Tick : HediffCompHandler_SecondaryCondition, IHediffComp_SecondaryCondition_TickHandler
{
    // don't rename this field. XML defs depend on this name
    private readonly int tickInterval = GenTicks.TickRareInterval;

    public int TickInterval => tickInterval;

    public override bool ShouldSkip(HediffComp_SecondaryCondition comp) => !comp.Pawn.IsHashIntervalTick(TickInterval) || base.ShouldSkip(comp);

    public virtual void Tick(HediffComp_SecondaryCondition comp)
    {
        if (!ShouldSkip(comp))
        {
            Evaulate(comp);
        }
    }
}
