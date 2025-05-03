namespace MoreInjuries.Caching.Threading;

/// <summary>
/// Common timeout tracker.
/// </summary>
internal readonly struct TimeoutTracker
{
    private readonly int _total;
    private readonly int _start;

    public TimeoutTracker(TimeSpan timeout)
    {
        long ltm = (long)timeout.TotalMilliseconds;

        _total = (int)ltm;
        if (!IsZeroOrInfinite)
        {
            _start = Environment.TickCount;
        }
    }

    public TimeoutTracker(int millisecondsTimeout)
    {
        _total = millisecondsTimeout;
        if (!IsZeroOrInfinite)
        {
            _start = Environment.TickCount;
        }
    }

    public int RemainingMilliseconds
    {
        get
        {
            if (IsZeroOrInfinite)
            {
                return _total;
            }
            int elapsed = Environment.TickCount - _start;
            // elapsed may be negative if TickCount has overflowed by 2^31 milliseconds.
            if (elapsed < 0 || elapsed >= _total)
            {
                return 0;
            }
            return _total - elapsed;
        }
    }

    private readonly bool IsZeroOrInfinite => _total is 0 or -1;

    public bool IsExpired => RemainingMilliseconds == 0;
}
