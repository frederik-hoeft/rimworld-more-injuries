using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Collections.Generic;

namespace MoreInjuries.Caching;

public sealed class TimedDataCache<TOwner, TData, TState, TCacheEntry>(int minCacheRefreshIntervalTicks, Func<TOwner, TState, TData> dataProvider) 
    : TimedDataCacheBase<TOwner, TData, TState, TCacheEntry>(minCacheRefreshIntervalTicks, dataProvider)
    where TOwner : class
    where TCacheEntry : class, ITimedDataEntry<TData>, new()
{
    private readonly Dictionary<TOwner, TCacheEntry> _cache = [];

    public override void Clear()
    {
        lock (_lock)
        {
            _cache.Clear();
        }
    }

    public override bool RemoveData(TOwner key)
    {
        Throw.ArgumentNullException.IfNull(key);
        lock (_lock)
        {
            return _cache.Remove(key);
        }
    }

    protected override void Add(TOwner owner, TCacheEntry entry) => 
        _cache.Add(owner, entry);

    protected override bool TryGetValue(TOwner owner, [NotNullWhen(true)] out TCacheEntry? entry) => 
        _cache.TryGetValue(owner, out entry);
}
