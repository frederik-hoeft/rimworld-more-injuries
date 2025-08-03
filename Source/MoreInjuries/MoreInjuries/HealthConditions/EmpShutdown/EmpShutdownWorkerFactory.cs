namespace MoreInjuries.HealthConditions.EmpShutdown;

public sealed class EmpShutdownWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new EmpShutdownWorker(parent);
}
