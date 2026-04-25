using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.Extensions;

internal static class SymbolExtensions
{
    extension(ISymbol self)
    {
        /// <summary>
        /// Determines whether the current object has an attribute of the specified type.
        /// </summary>
        /// <remarks>This method iterates through the attributes associated with the current object and
        /// checks for the presence of the specified attribute type. It performs a case-sensitive comparison of the
        /// attribute type's full name.</remarks>
        /// <typeparam name="T">The type of the attribute to check for. This type must derive from the base class Attribute.</typeparam>
        /// <returns>true if the specified attribute type is present; otherwise, false.</returns>
        public bool HasAttribute<T>() where T : Attribute => self.TryGetAttribute<T>(out _);

        /// <summary>
        /// Attempts to retrieve the first attribute of the specified type applied to the current symbol.
        /// </summary>
        /// <remarks>Use this method to efficiently check for and retrieve a specific attribute without
        /// throwing an exception if the attribute is not present. The search is case-sensitive and matches the full
        /// metadata name of the attribute type.</remarks>
        /// <typeparam name="T">The type of attribute to search for. Must derive from Attribute.</typeparam>
        /// <param name="attributeData">When this method returns, contains the AttributeData for the first matching attribute if found; otherwise,
        /// null. This parameter is passed uninitialized.</param>
        /// <returns>true if an attribute of type T is found; otherwise, false.</returns>
        public bool TryGetAttribute<T>([NotNullWhen(true)] out AttributeData? attributeData) where T : Attribute
        {
            foreach (AttributeData attribute in self.GetAttributes())
            {
                if (attribute.AttributeClass is { } attributeClass && attributeClass.GetFullMetadataName().Equals(typeof(T).FullName, StringComparison.Ordinal))
                {
                    attributeData = attribute;
                    return true;
                }
            }
            attributeData = null;
            return false;
        }
    }
}