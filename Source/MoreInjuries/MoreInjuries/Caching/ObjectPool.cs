using MoreInjuries.Caching.Threading;
using System.Threading;

namespace MoreInjuries.Caching;

/// <summary>
/// A pool that attempts to reuse objects, allocating new ones only when the pool is empty.
/// </summary>
/// <remarks>
/// This pool is thread safe and prioritizes returning over renting.
/// </remarks>
/// <typeparam name="T">The type of the object to pool.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
/// </remarks>
/// <param name="maxCapacity">The maximum number of objects to pool.</param>
/// <exception cref="ArgumentOutOfRangeException"><paramref name="maxCapacity"/> is less than or equal to zero.</exception>
public sealed class ObjectPool<T>(int maxCapacity, Func<IPool<T>, T> factory) : IPool<T>, IDisposable where T : class
{
    private readonly T?[] _pool = new T[maxCapacity];

    // Renting is thread safe with any concurrent number of renters, and returning is also thread safe with any number of concurrent returners,
    // but interleaving of renting and returning is a serios issue. As such, we use an alpha-beta lock to ensure that only one type of the two operations
    // can be performed at a time, possibly with multiple concurrent operations of the same type.
    // returning takes precedence over renting, so we use the beta lock for renting and the alpha lock for returning.
    // this will ensure that elements are reused as much as possible.
    private readonly AlphaBetaLockSlim _abls = new();

    /// <summary>
    /// Points to the next free index in the array.
    /// </summary>
    private int _index = 0;
    private bool _disposedValue;

    /// <inheritdoc/>
    public int MaxCapacity => _pool.Length;

    /// <inheritdoc/>
    public int Count => Volatile.Read(ref _index);

    /// <inheritdoc/>
    public T Rent()
    {
        // returning takes precedence over renting, so we use the beta lock here
        using ILockOwnership betaLock = _abls.AcquireBetaLock();
        int original = Atomic.DecrementClampMinFast(ref _index, 0);
        int myIndex = original - 1;
        if (myIndex < 0)
        {
            return factory.Invoke(this);
        }
        T element = Interlocked.Exchange(ref _pool[myIndex], null)!;
        return element;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This method does not guarantee that the object will be successfully returned to the pool, even if the pool is not full.
    /// </remarks>
    public bool Return(T item)
    {
        // acquire the alpha lock to ensure that no other thread is renting while we (and possibly other threads) are returning
        using ILockOwnership alphaLock = _abls.AcquireAlphaLock();
        int myIndex = Atomic.IncrementClampMaxFast(ref _index, _pool.Length - 1);
        if (myIndex + 1 < _pool.Length)
        {
            T? old = Interlocked.CompareExchange(ref _pool[myIndex], item, null);
            Std::Diagnostics.Debug.Assert(old is null, "The pool should never contain null elements on a non-empty index.");
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposedValue)
        {
            foreach (T? item in _pool)
            {
                if (item is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _disposedValue = true;
        }
    }
}