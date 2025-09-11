using MoreInjuries.Extensions;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions;

public abstract class InjuryWorker(MoreInjuryComp parent) : IInjuryHandler
{
    public abstract bool IsEnabled { get; }

    internal protected MoreInjuryComp Parent { get; } = parent;

    internal protected Pawn Pawn => Parent.Pawn;

    public bool PatientIsActivelyHostileTo(Pawn other) => Pawn.IsActivelyHostileTo(other);
}
