namespace MoreInjuries.HealthConditions.HeavyBleeding;

public sealed class HeavyBleedingWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new HeavyBleedingWorker(parent);
}
