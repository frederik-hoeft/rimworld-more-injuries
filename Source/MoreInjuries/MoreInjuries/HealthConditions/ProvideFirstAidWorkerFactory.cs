namespace MoreInjuries.HealthConditions;

public sealed class ProvideFirstAidWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new ProvideFirstAidWorker(parent);
}