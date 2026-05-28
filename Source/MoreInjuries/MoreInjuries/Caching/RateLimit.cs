using Verse;

namespace MoreInjuries.Caching;

public sealed class RateLimit(int minInterval) : ITimedCache
{
    private readonly TimedEntry _entry = new();

    public int MinInterval => minInterval;

    int ITimedCache.MinRefreshIntervalTicks => MinInterval;

    public void Reset() => _entry.Expire();

    public void ForceEnter() => _entry.Initialize(GenTicks.TicksGame);

    public bool CanEnter() => _entry.IsExpired(this, GenTicks.TicksGame);

    public bool TryEnter()
    {
        int currentTicks = GenTicks.TicksGame;
        if (_entry.IsExpired(this, currentTicks))
        {
            _entry.Initialize(currentTicks);
            return true;
        }
        return false;
    }
}
