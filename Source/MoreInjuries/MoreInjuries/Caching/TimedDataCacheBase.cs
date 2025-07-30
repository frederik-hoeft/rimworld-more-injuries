using MoreInjuries.Roslyn.Future.ThrowHelpers;
using Verse;

namespace MoreInjuries.Caching;

public abstract class TimedDataCacheBase<TOwner, TData, TState, TCacheEntry>
(
    int minCacheRefreshIntervalTicks,
    Func<TOwner, TState, TData> dataProvider
) : IDataCache<TOwner, TData, TState>
    where TOwner : class
    where TCacheEntry : class, ITimedDataEntry<TData>, new()
{
    private protected readonly object _lock = new();
    
    public int MinCacheRefreshIntervalTicks => minCacheRefreshIntervalTicks;

    protected abstract bool TryGetValue(TOwner owner, [NotNullWhen(true)] out TCacheEntry? entry);

    protected abstract void Add(TOwner owner, TCacheEntry entry);

    public TData GetData(TOwner owner, TState state, bool forceRefresh = false)
    {
        Throw.ArgumentNullException.IfNull(owner);
        lock (_lock)
        {
            if (TryGetValue(owner, out TCacheEntry? entry))
            {
                if (!forceRefresh && !IsExpired(entry) && entry.Data is TData materializedData)
                {
                    // if the entry is not expired, return the cached data
                    return materializedData;
                }
                // need to refresh the entry
                TData newData = dataProvider.Invoke(owner, state);
                entry.Initialize(newData, Find.TickManager.TicksGame);
                return newData;
            }
            // create a new entry if it does not exist
            TData data = dataProvider.Invoke(owner, state);
            entry = new TCacheEntry();
            entry.Initialize(data, Find.TickManager.TicksGame);
            Add(owner, entry);
            return data;
        }
    }

    public abstract bool RemoveData(TOwner key);

    public abstract void Clear();

    protected bool IsExpired(TCacheEntry entry) =>
        // check if the entry is expired based on the current game ticks
        entry.TimeStamp + MinCacheRefreshIntervalTicks < Find.TickManager.TicksGame;
}
