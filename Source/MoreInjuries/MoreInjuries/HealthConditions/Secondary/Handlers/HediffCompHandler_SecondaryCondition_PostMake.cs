using MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

public class HediffCompHandler_SecondaryCondition_PostMake : HediffCompHandler_SecondaryCondition, IHediffComp_SecondaryCondition_PostMakeHandler
{
    public virtual void PostMake(HediffComp_SecondaryCondition comp)
    {
        HediffMakerDef hediffMakerDef = HediffMakerProps.GetHediffMakerDef(comp, handler: this);
        if (!ShouldSkip(comp, hediffMakerDef.HediffDef))
        {
            // Evaluate the comp immediately after it is created
            Evaluate(comp, hediffMakerDef);
        }
    }
}
