using MoreInjuries.Roslyn.Future.ThrowHelpers;

namespace MoreInjuries.Caching;

internal sealed class WeakTimedDataEntry<TData> : ITimedDataEntry<TData> where TData : class
{
    private WeakReference<TData>? _data;

    public TData? Data
    {
        get => _data?.TryGetTarget(out TData? target) is true ? target : null; 
        private set => _data = value is not null ? new WeakReference<TData>(value) : null;
    }

    public int TimeStamp { get; private set; } = -1;

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
}