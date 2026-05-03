using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

/// <summary>
/// Provides throw helpers used by the XML serialization source generator to validate
/// that XML-bound backing fields have been initialized. This class is not intended
/// for direct use in application code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class XmlFieldThrowHelper
{
    [DoesNotReturn]
    private static void ThrowNull(string typeName, string? memberExpression) =>
        throw new InvalidOperationException(
            $"XML-bound member '{memberExpression}' of type '{typeName}' was not initialized. " +
            "Ensure the XML def provides a value for this field.");

    [DoesNotReturn]
    private static void ThrowNullOrEmpty(string typeName, string? memberExpression) =>
        throw new InvalidOperationException(
            $"XML-bound member '{memberExpression}' of type '{typeName}' was not initialized or is empty. " +
            "Ensure the XML def provides a non-empty value for this field.");

    /// <summary>
    /// Returns <paramref name="value"/> if it is not <see langword="null"/>; otherwise throws
    /// an <see cref="InvalidOperationException"/> indicating that the XML-bound field was not initialized.
    /// </summary>
    [return: NotNull]
    public static T NotNull<T>([NotNull] T? value, string typeName, [CallerArgumentExpression(nameof(value))] string? expression = null) where T : class
    {
        if (value is null)
        {
            ThrowNull(typeName, expression);
        }
        return value;
    }

    /// <summary>
    /// Returns <paramref name="value"/> if it is not <see langword="null"/> or empty; otherwise throws
    /// an <see cref="InvalidOperationException"/> indicating that the XML-bound field was not initialized.
    /// </summary>
    [return: NotNull]
    public static string NotNullOrEmpty([NotNull] string? value, string typeName, [CallerArgumentExpression(nameof(value))] string? expression = null)
    {
        if (value is null or { Length: 0 })
        {
            ThrowNullOrEmpty(typeName, expression);
        }
        return value;
    }
}
