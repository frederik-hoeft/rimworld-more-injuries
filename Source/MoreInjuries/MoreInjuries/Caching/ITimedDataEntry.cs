namespace MoreInjuries.Caching;

public interface ITimedDataEntry<TData> : ITimedEntry
{
    /// <summary>
    /// Gets the data stored in this cache entry.
    /// </summary>
    TData? Data { get; }

    /// <summary>
    /// Initializes this cache entry with the given data and current timestamp.
    /// </summary>
    [MemberNotNull(nameof(Data))]
    void Initialize(TData data, int currentTimeStamp);

    void Clear();
}
