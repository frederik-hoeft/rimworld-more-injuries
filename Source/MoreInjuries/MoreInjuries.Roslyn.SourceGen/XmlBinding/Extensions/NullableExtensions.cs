namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

internal static class NullableExtensions
{
    extension<T>(T value) where T : struct
    {
        public T? AsNullable() => value;
    }
}
