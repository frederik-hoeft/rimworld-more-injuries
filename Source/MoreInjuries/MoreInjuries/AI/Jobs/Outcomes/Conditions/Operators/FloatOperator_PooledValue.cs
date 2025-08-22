using MoreInjuries.Caching;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;

internal sealed class FloatOperator_PooledValue(IPool<FloatOperator_PooledValue> pool) : FloatOperator, IDisposable
{
    private static readonly ObjectPool<FloatOperator_PooledValue> s_constantPool = new(maxCapacity: 16, factory: static pool => new FloatOperator_PooledValue(pool));

    private bool _pooled = true;

    public float Value { get; set; }

    public static FloatOperator_PooledValue Rent()
    {
        FloatOperator_PooledValue instance = s_constantPool.Rent();
        instance.Initialize();
        return instance;
    }

    private void Initialize() => _pooled = false;

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) => Value;

    public override string ToString() => $"{nameof(FloatOperator_PooledValue)}: {Value}. You should never see this message, since this type is internal";

    public void Dispose()
    {
        Throw.InvalidOperationException.If(_pooled);
        _pooled = true;
        Value = default;
        pool.Return(this);
    }
}
