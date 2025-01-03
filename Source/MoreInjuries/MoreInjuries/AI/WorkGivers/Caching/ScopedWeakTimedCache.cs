using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace MoreInjuries.AI.WorkGivers.Caching;

public abstract class ScopedWeakTimedCache<TThing> where TThing : Thing
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
            Logger.LogDebug($"Loaded {cache.Count} weak references to things of type {typeof(TThing).Name} on map {map.uniqueID} from cache");
            List<int>? deadReferences = null;
            foreach ((int hashCode, Std::WeakReference<TThing> weakRef) in cache)
            {
                if (weakRef.TryGetTarget(out TThing? target))
                {
                    yield return target;
                }
                else
                {
                    Logger.LogDebug($"Removing dead reference for {typeof(TThing).Name} on map {map.uniqueID}");
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
        Logger.LogDebug($"Checking cache for {typeof(TThing).Name} on map {map.uniqueID}");
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
        Logger.LogDebug($"Refreshing cache for {typeof(TThing).Name} on map {map.uniqueID}");
        if (!_mapThingCache.TryGetValue(map, out cache))
        {
            Logger.LogDebug($"Creating new cache for {typeof(TThing).Name} on map {map.uniqueID}");
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
        Logger.LogDebug($"Cached {cache.Count} instances of {typeof(TThing).Name} on map {map.uniqueID}");
    }
}
