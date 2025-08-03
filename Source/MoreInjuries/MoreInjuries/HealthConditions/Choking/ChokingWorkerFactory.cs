namespace MoreInjuries.HealthConditions.Choking;

public sealed class ChokingWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new ChokingWorker(parent);
}
