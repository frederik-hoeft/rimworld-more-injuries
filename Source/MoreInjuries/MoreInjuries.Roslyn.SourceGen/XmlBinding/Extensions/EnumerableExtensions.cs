namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

internal static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> source)
        where T : struct
    {
        public T? FirstOrNull()
        {
            foreach (T item in source)
            {
                return item.AsNullable();
            }

            return null;
        }
    }
}
