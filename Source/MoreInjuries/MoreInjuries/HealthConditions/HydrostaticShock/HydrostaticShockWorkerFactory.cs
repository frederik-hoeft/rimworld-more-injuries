namespace MoreInjuries.HealthConditions.HydrostaticShock;

public sealed class HydrostaticShockWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new HydrostaticShockWorker(parent);
}
