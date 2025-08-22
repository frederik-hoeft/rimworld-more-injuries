namespace MoreInjuries.HealthConditions.HearingLoss;

public sealed class HearingLossExplosionsWorkerFactory : IInjuryWorkerFactory
{
    public InjuryWorker Create(MoreInjuryComp parent) => new HearingLossExplosionsWorker(parent);
}
