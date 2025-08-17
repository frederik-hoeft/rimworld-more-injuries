using MoreInjuries.Roslyn.Future.ThrowHelpers;

namespace MoreInjuries.Caching;

internal sealed class TimedDataEntry<TData> : ITimedDataEntry<TData>
{
    public TData? Data { get; private set; }

    public int TimeStamp { get; private set; } = -1;

    public void Clear()
    {
        Data = default;
        TimeStamp = -1;
    }

    [MemberNotNull(nameof(Data))]
    public void Initialize(TData data, int currentTimeStamp)
    {
        Throw.ArgumentNullException.IfNull(data);
        Throw.ArgumentOutOfRangeException.IfNegative(currentTimeStamp);
        if (currentTimeStamp == 0)
        {
            // may happen during game initialization, but is unexpected otherwise
            Logger.Warning("Initializing with zero timestamp. Unless you are currently creating a new game, please report this as a bug.");
        }
        Data = data;
        TimeStamp = currentTimeStamp;
    }

    public bool IsExpired(ITimedCache cache, int currentTimeStamp) => TimeStamp == -1 || TimeStamp + cache.MinRefreshIntervalTicks < currentTimeStamp;
}