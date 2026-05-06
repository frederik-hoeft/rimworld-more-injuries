namespace MoreInjuries.HealthConditions.Secondary.Handlers;

public class HediffCompHandler_SecondaryCondition_PostMake : HediffCompHandler_SecondaryCondition, IHediffComp_SecondaryCondition_PostMakeHandler
{
    public virtual void PostMake(HediffComp_SecondaryCondition comp)
    {
        if (!ShouldSkip(comp))
        {
            // Evaluate the comp immediately after it is created
            Evaulate(comp);
        }
    }
}
