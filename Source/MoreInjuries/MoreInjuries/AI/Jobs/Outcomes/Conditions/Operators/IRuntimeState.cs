namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;

public interface IRuntimeState : IDisposable
{
    bool TryResolve(string symbol, out float value);

    void Assign(string symbol, float value);
}
