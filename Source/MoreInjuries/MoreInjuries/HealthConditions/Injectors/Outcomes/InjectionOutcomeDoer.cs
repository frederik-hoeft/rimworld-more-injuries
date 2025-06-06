using MoreInjuries.BuildIntrinsics;
using MoreInjuries.HealthConditions.Injectors.Outcomes.Conditions;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Injectors.Outcomes;

[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public abstract class InjectionOutcomeDoer
{
    // don't rename this field. XML defs depend on this name
    private readonly OutcomeDoerCondition? condition = null;

    public OutcomeDoerCondition? Condition => condition;

    public virtual bool TryDoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        if (Condition is null || Condition.ShouldDoOutcome(doctor, patient, device))
        {
            return DoOutcome(doctor, patient, device);
        }
        return true; // condition not met, outcome not applied => no error
    }

    protected abstract bool DoOutcome(Pawn doctor, Pawn patient, Thing? device);
}