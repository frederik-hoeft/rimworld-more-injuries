namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;

public interface IRuntimeState : IDisposable
{
    bool TryResolve(string symbol, out float value);

    float ResolveRequired(string symbol);

    void Assign(string symbol, float value);
}
