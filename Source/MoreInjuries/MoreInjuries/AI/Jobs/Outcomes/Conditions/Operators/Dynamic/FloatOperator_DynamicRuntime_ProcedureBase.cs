using MoreInjuries.Caching;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

public abstract class FloatOperator_DynamicRuntime_ProcedureBase : FloatOperator
{
    private static readonly ObjectPool<RuntimeState> s_runtimePool = new(maxCapacity: 16, factory: static pool => new RuntimeState(pool));
    private List<FloatOperator>? _instructions;

    private List<FloatOperator> Instructions => _instructions ??= LoadInstructions();

    protected abstract List<FloatOperator> LoadInstructions();

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        if (Instructions is not { Count: > 0 } instructions)
        {
            throw new InvalidOperationException($"{nameof(FloatOperator_DynamicRuntime_ProcedureBase)}: statements cannot be null or empty");
        }
        bool ownsRuntimeState = false;
        if (runtimeState is null)
        {
            ownsRuntimeState = true;
            RuntimeState state = s_runtimePool.Rent();
            state.Initialize();
            runtimeState = state;
        }
        float lastResult = 0f;
        foreach (FloatOperator statement in instructions)
        {
            lastResult = statement.Evaluate(doctor, patient, device, runtimeState);
        }
        if (ownsRuntimeState)
        {
            runtimeState.Dispose();
        }
        return lastResult;
    }

    private sealed class RuntimeState(IPool<RuntimeState> pool) : IRuntimeState
    {
        private bool _pooled;

        private readonly Dictionary<string, float> _symbolTable = [];

        public void Assign(string symbol, float value)
        {
            Throw.ObjectDisposedException.If(_pooled, this);
            _symbolTable[symbol] = value;
        }

        public bool TryResolve(string symbol, out float value)
        {
            Throw.ObjectDisposedException.If(_pooled, this);
            if (_symbolTable.TryGetValue(symbol, out value))
            {
                return true;
            }
            value = default;
            return false;
        }

        public void Initialize() => _pooled = false;

        public void Dispose()
        {
            _symbolTable.Clear();
            _pooled = true;
            pool.Return(this);
        }
    }
}