namespace MoreInjuries.HealthConditions.HeadInjury.Concussions;

public sealed class ConcussionExplosionsWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new ConcussionExplosionsWorker(parent);
}
