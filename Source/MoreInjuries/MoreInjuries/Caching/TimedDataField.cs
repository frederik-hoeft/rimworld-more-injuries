using Verse;

namespace MoreInjuries.Caching;

public sealed class TimedDataField<TOwner, TData, TCacheEntry>(TOwner owner, int minRefreshIntervalTicks, Func<TOwner, TData> dataProvider) 
    : TimedDataField<TOwner, TData, object?, TCacheEntry>(owner, minRefreshIntervalTicks, (owner, _) => dataProvider.Invoke(owner))
    where TOwner : class
    where TCacheEntry : class, ITimedDataEntry<TData>, new ()
{
    public TData GetData(bool forceRefresh = false) =>
        GetData(state: null, forceRefresh);
}

public class TimedDataField<TOwner, TData, TState, TCacheEntry>(TOwner owner, int minRefreshIntervalTicks, Func<TOwner, TState, TData> dataProvider) : ITimedCache
    where TOwner : class
    where TCacheEntry : class, ITimedDataEntry<TData>, new()
{
    private TCacheEntry? _timedDataEntry;

    public int MinRefreshIntervalTicks => minRefreshIntervalTicks;

    public virtual TData GetData(TState state, bool forceRefresh = false)
    {
        int currentTicks = Find.TickManager.TicksGame;
        TCacheEntry entry = _timedDataEntry ??= new TCacheEntry();
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

    public virtual void Clear() => _timedDataEntry?.Clear();
}