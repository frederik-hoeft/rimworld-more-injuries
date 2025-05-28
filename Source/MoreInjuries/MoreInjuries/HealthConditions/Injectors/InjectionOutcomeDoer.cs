using Verse;

namespace MoreInjuries.HealthConditions.Injectors;

public abstract class InjectionOutcomeDoer
{
    public abstract bool TryDoOutcome(Pawn doctor, Pawn patient, Thing? device);
}
