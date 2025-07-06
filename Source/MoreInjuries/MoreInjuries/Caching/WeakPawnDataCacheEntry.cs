namespace MoreInjuries.Caching;

internal sealed class WeakPawnDataCacheEntry<TData>(TData data)
{
    public TData Data { get; set; } = data;
}