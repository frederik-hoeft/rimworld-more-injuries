using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace MoreInjuries.Roslyn.SourceGen.Extensions;

/// <summary>
/// Helper to load embedded source text from this assembly.
/// </summary>
internal static class SourceTextExtensions
{
    extension(SourceText)
    {
        public static SourceText FromEmbedded<T>() => FromEmbedded(typeof(T));

        public static SourceText FromEmbedded(Type type)
        {
            if (type.Assembly != typeof(SourceTextExtensions).Assembly)
            {
                throw new InvalidOperationException("The type T must be defined in the same assembly as SourceTextExtensions.");
            }
            string typeName = type.FullName ?? throw new InvalidOperationException("The type must have a full name.");
            using Stream stream = typeof(SourceTextExtensions).Assembly.GetManifestResourceStream($"{typeName}.cs")
                ?? throw new InvalidOperationException($"The embedded resource '{typeName}.cs' was not found.");
            return SourceText.From(stream, Encoding.UTF8, canBeEmbedded: true);
        }

        public static SourceText FromEmbedded<T>(string typeName) => FromEmbedded(typeof(T), typeName);

        public static SourceText FromEmbedded(Type type, string typeName)
        {
            using Stream stream = typeof(SourceTextExtensions).Assembly.GetManifestResourceStream($"{type.Namespace}.{typeName}.cs")
                ?? throw new InvalidOperationException($"The embedded resource '{type}.{typeName}.cs' was not found.");
            return SourceText.From(stream, Encoding.UTF8, canBeEmbedded: true);
        }
    }
}
