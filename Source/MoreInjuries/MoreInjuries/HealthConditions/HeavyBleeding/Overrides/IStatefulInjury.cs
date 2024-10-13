namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides;

public interface IStatefulInjury
{
    IInjuryState State { get; }
}
