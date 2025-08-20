namespace MoreInjuries.Caching;

public interface ITimedCache
{
    int MinRefreshIntervalTicks { get; }
}
