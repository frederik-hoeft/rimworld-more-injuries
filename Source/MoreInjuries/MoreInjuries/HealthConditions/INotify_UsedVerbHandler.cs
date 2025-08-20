using Verse;

namespace MoreInjuries.HealthConditions;

public interface INotify_UsedVerbHandler : IInjuryHandler
{
    void Notify_UsedVerb(Pawn pawn, Verb verb);
}