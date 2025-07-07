namespace MoreInjuries.HealthConditions.CardiacArrest;

public sealed class CardiacArrestWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new CardiacArrestWorker(parent);
}
