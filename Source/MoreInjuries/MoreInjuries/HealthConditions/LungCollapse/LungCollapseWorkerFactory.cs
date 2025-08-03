namespace MoreInjuries.HealthConditions.LungCollapse;

public sealed class LungCollapseWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new LungCollapseWorker(parent);
}
