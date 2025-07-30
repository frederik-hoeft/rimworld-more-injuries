using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Runtime.CompilerServices;

namespace MoreInjuries.Caching;

public sealed class WeakDataCache<TWeakOwner, TData>(Func<TWeakOwner, TData> dataProvider) 
    : WeakDataCache<TWeakOwner, TData, object?>((owner, _) => dataProvider.Invoke(owner)) where TWeakOwner : class
{
    public TData GetData(TWeakOwner owner, bool forceRefresh = false) => 
        GetData(owner, state: null, forceRefresh);
}

public class WeakDataCache<TWeakOwner, TData, TState>(Func<TWeakOwner, TState, TData> dataProvider) : IDataCache<TWeakOwner, TData, TState> where TWeakOwner : class
{
    private readonly object _lock = new();
    private readonly ConditionalWeakTable<TWeakOwner, WeakDataEntry<TData>> _cache = [];

    public void Clear() => _cache.Clear();

    public TData GetData(TWeakOwner owner, TState state, bool forceRefresh)
    {
        Throw.ArgumentNullException.IfNull(owner);
        lock (_lock)
        {
            if (_cache.TryGetValue(owner, out WeakDataEntry<TData>? entry))
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
                TData newData = dataProvider.Invoke(owner, state);
                entry.Data = newData;
                return newData;
            }
            // create a new entry if it does not exist
            TData data = dataProvider.Invoke(owner, state);
            entry = new WeakDataEntry<TData>(data);
            _cache.Add(owner, entry);
            return data;
        }
    }

    public bool RemoveData(TWeakOwner key)
    {
        Throw.ArgumentNullException.IfNull(key);
        return _cache.Remove(key);
    }
}