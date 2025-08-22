using MoreInjuries.Roslyn.Future.ThrowHelpers;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

public abstract class FloatOperator_AssignBase(string? symbol) : FloatOperator_MemoryBase(symbol)
{
    protected float AssignValue(float value, [NotNull] IRuntimeState? runtimeState)
    {
        Throw.ArgumentNullException.IfNull(runtimeState);
        runtimeState.Assign(Symbol, value);
        return value;
    }

    protected abstract string ValueToString();

    public override string ToString() => $"({Symbol} = {ValueToString()})";
}
