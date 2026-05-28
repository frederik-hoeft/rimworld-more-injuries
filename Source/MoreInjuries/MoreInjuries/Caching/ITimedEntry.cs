namespace MoreInjuries.Caching;

public interface ITimedEntry
{
    int TimeStamp { get; }

    bool IsExpired(ITimedCache cache, int currentTimeStamp);

    void Expire();
}
