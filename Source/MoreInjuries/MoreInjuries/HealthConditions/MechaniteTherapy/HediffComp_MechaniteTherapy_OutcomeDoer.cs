using MoreInjuries.Extensions;
using Verse;

namespace MoreInjuries.HealthConditions.MechaniteTherapy;

public abstract class HediffComp_MechaniteTherapy_OutcomeDoer : IExposable, ILoadReferenceable
{
    private string? _loadID;

    public abstract void DoOutcome(HediffComp_MechaniteTherapy parentComp);

    public virtual void ExposeData()
    {
        _loadID ??= GetUniqueLoadID();
        Scribe_Values.Look(ref _loadID, "loadID", null, false);
    }

    public string GetUniqueLoadID() => _loadID ??= this.GenerateUniqueLoadID();
}