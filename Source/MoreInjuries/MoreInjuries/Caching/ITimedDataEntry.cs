namespace MoreInjuries.Caching;

public interface ITimedDataEntry<TData>
{
    /// <summary>
    /// Gets the data stored in this cache entry.
    /// </summary>
    TData? Data { get; }

    /// <summary>
    /// Gets the timestamp of the data stored in this cache entry.
    /// </summary>
    int TimeStamp { get; }

    /// <summary>
    /// Initializes this cache entry with the given data and current timestamp.
    /// </summary>
    [MemberNotNull(nameof(Data))]
    void Initialize(TData data, int currentTimeStamp);
}
