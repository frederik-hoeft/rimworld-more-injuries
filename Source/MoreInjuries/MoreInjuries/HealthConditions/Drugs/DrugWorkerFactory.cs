namespace MoreInjuries.HealthConditions.Drugs;

public sealed class DrugWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new DrugWorker(parent);
}
