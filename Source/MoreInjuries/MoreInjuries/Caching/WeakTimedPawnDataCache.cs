using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Runtime.CompilerServices;
using Verse;

namespace MoreInjuries.Caching;

public sealed class WeakTimedPawnDataCache<TData>(int minCacheRefreshIntervalTicks, Func<Pawn, TData> dataProvider)
{
    private readonly object _lock = new();
    private readonly ConditionalWeakTable<Pawn, WeakTimedPawnDataCacheEntry<TData>> _cache = new();

    public int MinCacheRefreshIntervalTicks => minCacheRefreshIntervalTicks;

    public TData GetData(Pawn pawn, bool forceRefresh = false)
    {
        Throw.ArgumentNullException.IfNull(pawn);
        lock (_lock)
        {
            if (_cache.TryGetValue(pawn, out WeakTimedPawnDataCacheEntry<TData>? entry))
            {
                if (!forceRefresh && !IsExpired(entry))
                {
                    // if the entry is not expired, return the cached data
                    return entry.Data!;
                }
                // need to refresh the entry
                if (entry is { Data: IDisposable disposableData })
                {
                    // if the data is disposable, we dispose it before replacing
                    disposableData.Dispose();
                }
                TData newData = dataProvider.Invoke(pawn);
                entry.Initialize(newData, Find.TickManager.TicksGame);
                return newData;
            }
            // create a new entry if it does not exist
            TData data = dataProvider.Invoke(pawn);
            entry = new WeakTimedPawnDataCacheEntry<TData>();
            entry.Initialize(data, Find.TickManager.TicksGame);
            _cache.Add(pawn, entry);
            return data;
        }
    }

    private bool IsExpired(WeakTimedPawnDataCacheEntry<TData> entry) =>
        // check if the entry is expired based on the current game ticks
        entry.TimeStamp + MinCacheRefreshIntervalTicks < Find.TickManager.TicksGame;
}