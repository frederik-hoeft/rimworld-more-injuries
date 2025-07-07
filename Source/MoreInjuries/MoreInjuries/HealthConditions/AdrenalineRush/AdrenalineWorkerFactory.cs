namespace MoreInjuries.HealthConditions.AdrenalineRush;

public sealed class AdrenalineWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new AdrenalineWorker(parent);
}
