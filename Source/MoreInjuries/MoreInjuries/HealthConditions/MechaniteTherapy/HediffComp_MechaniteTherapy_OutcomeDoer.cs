using Verse;

namespace MoreInjuries.HealthConditions.MechaniteTherapy;

public abstract class HediffComp_MechaniteTherapy_OutcomeDoer : IExposable
{
    public abstract void DoOutcome(HediffComp_MechaniteTherapy parentComp);

    public virtual void ExposeData()
    {
        // override as needed
    }
}