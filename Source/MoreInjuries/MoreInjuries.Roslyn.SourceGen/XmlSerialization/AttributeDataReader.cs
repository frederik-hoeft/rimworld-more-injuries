using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

/// <summary>
/// Reads named and positional arguments from <see cref="AttributeData"/> instances.
/// </summary>
internal static class AttributeDataReader
{
    public static bool GetAllowRawAccess(AttributeData attribute) =>
        GetNamedBoolArgument(attribute, nameof(XmlMemberAttribute.AllowRawAccess));

    public static string? GetNamedStringArgument(AttributeData attribute, string argumentName)
    {
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == argumentName && namedArg.Value.Value is string { Length: > 0 } value)
            {
                return value;
            }
        }
        return null;
    }

    public static INamedTypeSymbol? GetNamedTypeArgument(AttributeData attribute, string argumentName)
    {
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == argumentName && namedArg.Value.Value is INamedTypeSymbol typeSymbol)
            {
                return typeSymbol;
            }
        }
        return null;
    }

    public static bool GetNamedBoolArgument(AttributeData attribute, string argumentName)
    {
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == argumentName && namedArg.Value.Value is true)
            {
                return true;
            }
        }
        return false;
    }
}
