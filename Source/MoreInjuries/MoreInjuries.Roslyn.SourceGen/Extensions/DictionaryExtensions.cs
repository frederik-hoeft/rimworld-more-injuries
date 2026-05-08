using System.Diagnostics.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.Extensions;

internal static class DictionaryExtensions
{
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        [return: NotNullIfNotNull(nameof(defaultValue))]
        public TValue? GetValueOrDefault(TKey key, TValue? defaultValue = default)
        {
            if (dictionary.TryGetValue(key, out TValue? value))
            {
                return value;
            }
            return defaultValue;
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }
            dictionary.Add(key, value);
            return true;
        }
    }
}
