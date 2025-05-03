using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace MoreInjuries.Caching;

public abstract class WeakTimedMapThingCache<TThing> where TThing : Thing
{
    private readonly ConditionalWeakTable<Map, Dictionary<int, Std::WeakReference<TThing>>> _mapThingCache = new();
    private int _lastRefreshTicks;

    protected abstract int MinCacheRefreshIntervalTicks { get; }

    protected abstract IEnumerable<TThing> GetMapThings(Map map);

    public IEnumerable<TThing> GetCachedThings(Map map)
    {
        TryRefreshCache(map, out Dictionary<int, Std::WeakReference<TThing>>? cache);
        if (cache is not null || _mapThingCache.TryGetValue(map, out cache))
        {
            List<int>? deadReferences = null;
            foreach ((int hashCode, Std::WeakReference<TThing> weakRef) in cache)
            {
                if (weakRef.TryGetTarget(out TThing? target))
                {
                    yield return target;
                }
                else
                {
                    deadReferences ??= [];
                    deadReferences.Add(hashCode);
                }
            }
            if (deadReferences is not null)
            {
                foreach (int hashCode in deadReferences)
                {
                    cache.Remove(hashCode);
                }
            }
        }
    }

    public bool HasCachedThings(Map map)
    {
        TryRefreshCache(map, out Dictionary<int, Std::WeakReference<TThing>>? cache);
        if (cache is null && !_mapThingCache.TryGetValue(map, out cache))
        {
            return false;
        }
        return cache.Count > 0;
    }

    private void TryRefreshCache(Map map, out Dictionary<int, Std::WeakReference<TThing>>? cache)
    {
        int ticks = Find.TickManager.TicksGame;
        // only refresh on the initial query or after the minimum interval has passed
        if (_lastRefreshTicks != 0 && ticks - _lastRefreshTicks < MinCacheRefreshIntervalTicks)
        {
            cache = null;
            return;
        }
        _lastRefreshTicks = ticks;
        if (!_mapThingCache.TryGetValue(map, out cache))
        {
            cache = [];
            _mapThingCache.Add(map, cache);
            return;
        }
        cache.Clear();
        foreach (TThing thing in GetMapThings(map))
        {
            int hashCode = thing.GetHashCode();
            cache[hashCode] = new Std::WeakReference<TThing>(thing);
        }
    }
}