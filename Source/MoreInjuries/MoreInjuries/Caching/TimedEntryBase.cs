namespace MoreInjuries.Caching;

public abstract class TimedEntryBase : ITimedEntry
{
    public int TimeStamp { get; protected set; } = -1;

    public virtual void Expire() => TimeStamp = -1;

    public virtual bool IsExpired(ITimedCache cache, int currentTimeStamp) =>
        TimeStamp == -1 || TimeStamp + cache.MinRefreshIntervalTicks < currentTimeStamp;
}
