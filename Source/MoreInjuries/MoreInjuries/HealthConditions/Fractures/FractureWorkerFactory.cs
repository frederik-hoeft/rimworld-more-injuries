namespace MoreInjuries.HealthConditions.Fractures;

public sealed class FractureWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new FractureWorker(parent);
}
