namespace MoreInjuries.Caching;

public sealed class ConcurrentTimedDataField<TOwner, TData, TCacheEntry>(TOwner owner, int minRefreshIntervalTicks, Func<TOwner, TData> dataProvider)
    : ConcurrentTimedDataField<TOwner, TData, object?, TCacheEntry>(owner, minRefreshIntervalTicks, (owner, _) => dataProvider.Invoke(owner))
    where TOwner : class
    where TCacheEntry : class, ITimedDataEntry<TData>, new()
{
    public TData GetData(bool forceRefresh = false) =>
        GetData(state: null, forceRefresh);
}

public class ConcurrentTimedDataField<TOwner, TData, TState, TCacheEntry>(TOwner owner, int minRefreshIntervalTicks, Func<TOwner, TState, TData> dataProvider)
    : TimedDataField<TOwner, TData, TState, TCacheEntry>(owner, minRefreshIntervalTicks, dataProvider)
    where TOwner : class
    where TCacheEntry : class, ITimedDataEntry<TData>, new()
{
    private readonly object _lock = new();

    public override TData GetData(TState state, bool forceRefresh = false)
    {
        lock (_lock)
        {
            return base.GetData(state, forceRefresh);
        }
    }

    public override void Clear()
    {
        lock (_lock)
        {
            base.Clear();
        }
    }
}