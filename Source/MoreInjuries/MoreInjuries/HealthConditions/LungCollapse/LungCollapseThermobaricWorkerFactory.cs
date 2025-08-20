namespace MoreInjuries.HealthConditions.LungCollapse;

public sealed class LungCollapseThermobaricWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new LungCollapseThermobaricWorker(parent);
}
