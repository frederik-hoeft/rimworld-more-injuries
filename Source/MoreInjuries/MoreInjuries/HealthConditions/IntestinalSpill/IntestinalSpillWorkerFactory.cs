namespace MoreInjuries.HealthConditions.IntestinalSpill;

internal sealed class IntestinalSpillWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new IntestinalSpillWorker(parent);
}
