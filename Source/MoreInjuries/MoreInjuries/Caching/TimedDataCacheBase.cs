using MoreInjuries.Roslyn.Future.ThrowHelpers;
using Verse;

namespace MoreInjuries.Caching;

public abstract class TimedDataCacheBase<TOwner, TData, TState, TCacheEntry>
(
    int minRefreshIntervalTicks,
    Func<TOwner, TState, TData> dataProvider
) : IDataCache<TOwner, TData, TState>, ITimedCache
    where TOwner : class
    where TCacheEntry : class, ITimedDataEntry<TData>, new()
{
    private protected readonly object _lock = new();
    
    public int MinRefreshIntervalTicks => minRefreshIntervalTicks;

    protected abstract bool TryGetValue(TOwner owner, [NotNullWhen(true)] out TCacheEntry? entry);

    protected abstract void Add(TOwner owner, TCacheEntry entry);

    public TData GetData(TOwner owner, TState state, bool forceRefresh = false)
    {
        Throw.ArgumentNullException.IfNull(owner);
        lock (_lock)
        {
            int currentTicks = Find.TickManager.TicksGame;
            if (TryGetValue(owner, out TCacheEntry? entry))
            {
                if (!forceRefresh && !entry.IsExpired(this, currentTicks) && entry.Data is TData materializedData)
                {
                    // if the entry is not expired, return the cached data
                    return materializedData;
                }
                // need to refresh the entry
                TData newData = dataProvider.Invoke(owner, state);
                entry.Initialize(newData, currentTicks);
                return newData;
            }
            // create a new entry if it does not exist
            TData data = dataProvider.Invoke(owner, state);
            entry = new TCacheEntry();
            entry.Initialize(data, currentTicks);
            Add(owner, entry);
            return data;
        }
    }

    public abstract bool RemoveData(TOwner key);

    public abstract void Clear();

    public virtual bool MarkDirty(TOwner owner)
    {
        Throw.ArgumentNullException.IfNull(owner);
        lock (_lock)
        {
            if (TryGetValue(owner, out TCacheEntry? entry))
            {
                entry.MarkDirty();
                return true;
            }
            return false;
        }
    }
}