using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Collections.Concurrent;

namespace MoreInjuries.Caching;

public sealed class DataCache<TOwner, TData>(Func<TOwner, TData> dataProvider) 
    : DataCache<TOwner, TData, object?>((owner, _) => dataProvider.Invoke(owner)) where TOwner : class
{
    public TData GetData(TOwner owner, bool forceRefresh = false) => 
        GetData(owner, state: null, forceRefresh);
}

public class DataCache<TOwner, TData, TState>(Func<TOwner, TState, TData> dataProvider) : IDataCache<TOwner, TData, TState> where TOwner : class
{
    private readonly ConcurrentDictionary<TOwner, TData> _cache = [];

    public void Clear() => _cache.Clear();

    public TData GetData(TOwner owner, TState state, bool forceRefresh = false)
    {
        Throw.ArgumentNullException.IfNull(owner);
        TData? newData;
        do
        {
            if (_cache.TryGetValue(owner, out TData? data))
            {
                if (!forceRefresh)
                {
                    return data;
                }
                // need to refresh the entry
            }
            // create a new entry if it does not exist or needs refresh
            newData = dataProvider.Invoke(owner, state);
        } while (!_cache.TryAdd(owner, newData));
        return newData;
    }

    public bool RemoveData(TOwner key)
    {
        Throw.ArgumentNullException.IfNull(key);
        return _cache.TryRemove(key, out _);
    }
}
