using MoreInjuries.Roslyn.Future.ThrowHelpers;

namespace MoreInjuries.Caching;

internal sealed class WeakTimedPawnDataCacheEntry<TData>
{
    public TData? Data { get; private set; }

    public int TimeStamp { get; private set; } = -1;

    [MemberNotNull(nameof(Data))]
    public void Initialize(TData data, int currentTimeStamp)
    {
        Throw.ArgumentNullException.IfNull(data);
        Throw.ArgumentOutOfRangeException.IfNegativeOrZero(currentTimeStamp, nameof(currentTimeStamp));
        Data = data;
        TimeStamp = currentTimeStamp;
    }
}