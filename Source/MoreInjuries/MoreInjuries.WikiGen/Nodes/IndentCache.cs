namespace MoreInjuries.WikiGen.Nodes;

internal sealed class IndentCache(int indentSize)
{
    private readonly Dictionary<int, string> _cache = [];

    public string GetIndent(int indentLevel)
    {
        if (!_cache.TryGetValue(indentLevel, out string? result))
        {
            result = new string(' ', indentLevel * indentSize);
            _cache.Add(indentLevel, result);
        }
        return result;
    }
}
