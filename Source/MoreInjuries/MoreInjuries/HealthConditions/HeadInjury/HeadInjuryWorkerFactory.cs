namespace MoreInjuries.HealthConditions.HeadInjury;

public sealed class HeadInjuryWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new HeadInjuryWorker(parent);
}
