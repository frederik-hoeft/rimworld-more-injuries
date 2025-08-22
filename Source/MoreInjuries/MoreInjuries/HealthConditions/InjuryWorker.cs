using Verse;

namespace MoreInjuries.HealthConditions;

public abstract class InjuryWorker(MoreInjuryComp parent) : IInjuryHandler
{
    public abstract bool IsEnabled { get; }

    internal protected MoreInjuryComp Parent { get; } = parent;

    internal protected Pawn Pawn => Parent.Pawn;
}
