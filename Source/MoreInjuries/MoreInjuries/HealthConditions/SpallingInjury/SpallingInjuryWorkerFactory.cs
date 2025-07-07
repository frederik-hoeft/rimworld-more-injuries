namespace MoreInjuries.HealthConditions.SpallingInjury;

public sealed class SpallingInjuryWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new SpallingInjuryWorker(parent);
}
