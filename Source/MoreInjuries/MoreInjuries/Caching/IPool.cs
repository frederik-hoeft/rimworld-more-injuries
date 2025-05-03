namespace MoreInjuries.Caching;

/// <summary>
/// Represents a pool of reusable items that can be rented and returned.
/// </summary>
/// <typeparam name="T">The type of the items in the pool.</typeparam>
public interface IPool<T>
{
    /// <summary>
    /// The maximum number of items that this pool can hold. 
    /// If more items are returned than this value, they will be ignored.
    /// If more items are rented than this value, they will be newly created.
    /// </summary>
    int MaxCapacity { get; }

    /// <summary>
    /// The number of items that are currently stored in this pool.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Returns a new item from the pool, or creates a new one if the pool is empty.
    /// </summary>
    /// <returns>A new item from the pool, or creates a new one if the pool is empty.</returns>
    T Rent();

    /// <summary>
    /// Returns the item to the pool. If the pool is full, the item will be ignored.
    /// </summary>
    /// <param name="item">The item to return to the pool.</param>
    /// <returns><see langword="true"/> if the item was returned to the pool, <see langword="false"/> if the pool is full.</returns>
    bool Return(T item);
}