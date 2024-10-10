using Verse;

namespace MoreInjuries.HealthConditions;

public abstract class InjuryWorker(MoreInjuryComp parent) : IInjuryHandler
{
    public abstract bool IsEnabled { get; }

    protected MoreInjuryComp Parent { get; } = parent;

    internal protected Pawn Target => (Pawn)Parent.parent;
}
