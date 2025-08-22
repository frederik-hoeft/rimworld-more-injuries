namespace MoreInjuries.Caching;

// un-poolable, since the owner is a weak reference, and we have no way to ensure correct release back to the pool, when the weak owner falls out of scope.
internal sealed class WeakDataEntry<TData>(TData data)
{
    public TData Data { get; set; } = data;
}