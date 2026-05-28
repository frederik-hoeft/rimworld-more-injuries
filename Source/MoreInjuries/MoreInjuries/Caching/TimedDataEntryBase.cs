namespace MoreInjuries.Caching;

public abstract class TimedDataEntryBase<TData> : TimedEntryBase, ITimedDataEntry<TData>
{
    public abstract TData? Data { get; protected set; }

    public abstract void Clear();

    public abstract void Initialize(TData data, int currentTimeStamp);
}
