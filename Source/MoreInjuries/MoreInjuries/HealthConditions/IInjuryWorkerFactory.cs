namespace MoreInjuries.HealthConditions;

public interface IInjuryWorkerFactory
{
    InjuryWorker Create(MoreInjuryComp parent);
}