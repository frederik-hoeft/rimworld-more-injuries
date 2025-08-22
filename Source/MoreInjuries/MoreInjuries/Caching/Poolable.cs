using MoreInjuries.Debug;
using MoreInjuries.Roslyn.Future.ThrowHelpers;

namespace MoreInjuries.Caching;

internal sealed class Poolable<T>(IPool<Poolable<T>> pool, Predicate<T>? canPool = null, Action<T>? reset = null, Func<T>? factory = null) : IDisposable
{
    private bool _isPooled = true;
    private T? _value;

    public T Value 
    {
        get
        {
            Throw.ObjectDisposedException.If(_isPooled, this);
            if (_value is not null)
            {
                return _value;
            }
            throw new InvalidOperationException("Cannot access the value of a Poolable object that was not initialized and has no factory.");
        }
    }

    public void Initialize()
    {
        DebugAssert.IsTrue(_isPooled, "Cannot initialize a Poolable object that was already initialized.");
        if (factory is not null)
        {
            _value = factory.Invoke();
        }
        _isPooled = false;
    }

    [MemberNotNull(nameof(_value))]
    public void Initialize(T value)
    {
        DebugAssert.IsTrue(_isPooled, "Cannot initialize a Poolable object that was already initialized.");
        Throw.ArgumentNullException.IfNull(value);
        _value = value;
    }

    public void Dispose()
    {
        T value = Value;
        if (canPool?.Invoke(value) is not true)
        {
            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }
            _value = default;
            return;
        }
        reset?.Invoke(value);
        _isPooled = true;
        pool.Return(this);
    }
}
