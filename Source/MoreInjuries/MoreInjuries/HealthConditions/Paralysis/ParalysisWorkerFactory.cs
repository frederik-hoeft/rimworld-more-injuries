namespace MoreInjuries.HealthConditions.Paralysis;

internal sealed class ParalysisWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new ParalysisWorker(parent);
}
