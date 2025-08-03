using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

public sealed class FloatOperator_Resolve() : FloatOperator_MemoryBase(symbol: null)
{
    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        Throw.ArgumentNullException.IfNull(runtimeState);
        if (runtimeState.TryResolve(Symbol, out float value))
        {
            return value;
        }
        throw new KeyNotFoundException($"{nameof(FloatOperator_Resolve)}: Symbol '{Symbol}' not found in runtime state");
    }

    public override string ToString() => Symbol;
}