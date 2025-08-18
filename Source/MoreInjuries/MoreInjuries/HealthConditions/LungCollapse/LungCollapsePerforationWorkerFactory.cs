namespace MoreInjuries.HealthConditions.LungCollapse;

public sealed class LungCollapsePerforationWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new LungCollapsePerforationWorker(parent);
}
