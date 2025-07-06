using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Runtime.CompilerServices;
using Verse;

namespace MoreInjuries.Caching;

public sealed class WeakPawnDataCache<TData>(Func<Pawn, TData> dataProvider)
{
    private readonly object _lock = new();
    private readonly ConditionalWeakTable<Pawn, WeakPawnDataCacheEntry<TData>> _cache = new();

    public TData GetData(Pawn pawn, bool forceRefresh)
    {
        Throw.ArgumentNullException.IfNull(pawn);
        lock (_lock)
        {
            if (_cache.TryGetValue(pawn, out WeakPawnDataCacheEntry<TData>? entry))
            {
                if (!forceRefresh)
                {
                    return entry.Data;
                }
                // need to refresh the entry
                if (entry is { Data: IDisposable disposableData })
                {
                    // if the data is disposable, we dispose it before replacing
                    disposableData.Dispose();
                }
                TData newData = dataProvider.Invoke(pawn);
                entry.Data = newData;
                return newData;
            }
            // create a new entry if it does not exist
            TData data = dataProvider.Invoke(pawn);
            entry = new WeakPawnDataCacheEntry<TData>(data);
            _cache.Add(pawn, entry);
            return data;
        }
    }
}