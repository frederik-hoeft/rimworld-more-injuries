namespace MoreInjuries.Caching;

public interface IDataCache<TKey, TData, TState> where TKey : notnull
{
    TData GetData(TKey key, TState state, bool forceRefresh = false);

    bool RemoveData(TKey key);

    void Clear();
}
