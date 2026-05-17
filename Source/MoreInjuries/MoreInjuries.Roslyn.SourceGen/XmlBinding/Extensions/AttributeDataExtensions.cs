using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

/// <summary>
/// Reads named and positional arguments from <see cref="AttributeData"/> instances.
/// </summary>
internal static class AttributeDataExtensions
{
    extension(AttributeData attribute)
    {
        public bool GetAllowRawAccess() => GetNamedArgumentValue(attribute, nameof(XmlBindingAttribute.AllowRawAccess)) is true;

        public string? GetNamedStringArgument(string argumentName) => GetNamedArgumentValue(attribute, argumentName) switch
        {
            string { Length: > 0 } value => value,
            _ => null,
        };

        public INamedTypeSymbol? GetNamedTypeArgument(string argumentName) =>
            GetNamedArgumentValue(attribute, argumentName) as INamedTypeSymbol;

        public bool GetNamedBoolArgument(string argumentName) =>
            GetNamedArgumentValue(attribute, argumentName) is true;

        private object? GetNamedArgumentValue(string argumentName) => attribute.NamedArguments
            .Where(argument => argument.Key == argumentName)
            .Select(static argument => argument.Value.Value)
            .FirstOrDefault();
    }
}
