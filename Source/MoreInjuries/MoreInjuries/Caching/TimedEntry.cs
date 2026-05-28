using MoreInjuries.Roslyn.Future.ThrowHelpers;

namespace MoreInjuries.Caching;

public sealed class TimedEntry : TimedEntryBase
{
    public void Initialize(int currentTimeStamp)
    {
        Throw.ArgumentOutOfRangeException.IfNegative(currentTimeStamp);
        if (currentTimeStamp == 0)
        {
            // may happen during game initialization, but is unexpected otherwise
            Logger.Warning("Initializing with zero timestamp. Unless you are currently creating a new game, please report this as a bug.");
        }
        TimeStamp = currentTimeStamp;
    }
}
