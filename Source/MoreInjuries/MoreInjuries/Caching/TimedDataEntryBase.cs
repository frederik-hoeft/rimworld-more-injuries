namespace MoreInjuries.Caching;

internal abstract class TimedDataEntryBase<TData> : ITimedDataEntry<TData>
{
    public int TimeStamp { get; protected set; } = -1;

    public abstract TData? Data { get; protected set; }

    public abstract void Clear();

    public abstract void Initialize(TData data, int currentTimeStamp);

    public virtual bool IsExpired(ITimedCache cache, int currentTimeStamp) => 
        TimeStamp == -1 || TimeStamp + cache.MinRefreshIntervalTicks < currentTimeStamp;

    public virtual void MarkDirty() => TimeStamp = -1;
}
