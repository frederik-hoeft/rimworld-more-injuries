namespace MoreInjuries.HealthConditions.InhalationInjury;

public sealed class InhalationInjuryWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new InhalationInjuryWorker(parent);
}
