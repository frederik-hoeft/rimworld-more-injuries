using Verse;

namespace MoreInjuries.HealthConditions;

public abstract class InjuryWorker(InjuryComp parent) : IInjuryHandler
{
    public abstract bool IsEnabled { get; }

    protected InjuryComp Parent { get; } = parent;

    protected Pawn Target => (Pawn)Parent.parent;
}
