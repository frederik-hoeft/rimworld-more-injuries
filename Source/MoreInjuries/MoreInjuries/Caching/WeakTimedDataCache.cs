using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Runtime.CompilerServices;

namespace MoreInjuries.Caching;

public sealed class WeakTimedDataCache<TWeakOwner, TData, TCacheEntry>(int minCacheRefreshIntervalTicks, Func<TWeakOwner, TData> dataProvider)
    : WeakTimedDataCache<TWeakOwner, TData, object?, TCacheEntry>(minCacheRefreshIntervalTicks, (owner, _) => dataProvider.Invoke(owner))
    where TWeakOwner : class
    where TCacheEntry : class, ITimedDataEntry<TData>, new()
{
    public TData GetData(TWeakOwner owner, bool forceRefresh = false) =>
        GetData(owner, state: null, forceRefresh);
}

public class WeakTimedDataCache<TWeakOwner, TData, TState, TCacheEntry>(int minCacheRefreshIntervalTicks, Func<TWeakOwner, TState, TData> dataProvider)
    : TimedDataCacheBase<TWeakOwner, TData, TState, TCacheEntry>(minCacheRefreshIntervalTicks, dataProvider)
    where TWeakOwner : class
    where TCacheEntry : class, ITimedDataEntry<TData>, new()
{
    private readonly ConditionalWeakTable<TWeakOwner, TCacheEntry> _cache = [];

    public override void Clear() => _cache.Clear();

    public override bool RemoveData(TWeakOwner key)
    {
        Throw.ArgumentNullException.IfNull(key);
        return _cache.Remove(key);
    }

    protected override void Add(TWeakOwner owner, TCacheEntry entry) => 
        _cache.Add(owner, entry);

    protected override bool TryGetValue(TWeakOwner owner, [NotNullWhen(true)] out TCacheEntry? entry) =>
        _cache.TryGetValue(owner, out entry);
}