namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

internal static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> source) where T : struct
    {
        public T? FirstAsNullable() => source.Cast<T?>().FirstOrDefault();
    }
}
