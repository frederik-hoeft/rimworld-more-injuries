using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace MoreInjuries.Roslyn.SourceGen.Extensions;

internal static class NamedTypeSymbolExtensions
{
    extension(INamedTypeSymbol self)
    {
        /// <summary>
        /// Enumerates the most derived members of the current type, optionally filtering by visibility context.
        /// </summary>
        /// <remarks>This method traverses the inheritance hierarchy from the current type to its base
        /// types, yielding only the most derived members that have not been previously encountered. Members that are
        /// implicitly declared or not visible in the given context are excluded from the results.</remarks>
        /// <param name="visibilityContext">An optional context that determines whether a member is accessible. If null, all members are considered
        /// visible.</param>
        /// <returns>An enumerable collection of symbols representing the most derived members that are visible according to the
        /// specified context.</returns>
        public IEnumerable<ISymbol> EnumerateMostDerivedMembers(VisibilityContext? visibilityContext = null)
        {
            HashSet<string> seenNames = new(StringComparer.Ordinal);

            for (INamedTypeSymbol? current = self; current is not null; current = current.BaseType)
            {
                foreach (ISymbol member in EnumerateMembersDeterministically(current))
                {
                    if (member.IsImplicitlyDeclared || member.HasAttribute<CompilerGeneratedAttribute>())
                    {
                        continue;
                    }
                    if (visibilityContext is not null && !visibilityContext.IsVisible(member))
                    {
                        continue;
                    }

                    // Most-derived visible member wins.
                    if (!seenNames.Add(member.Name))
                    {
                        continue;
                    }

                    yield return member;
                }
            }
        }

        /// <summary>
        /// Enumerates the members of the current symbol in a deterministic order based on their source location and
        /// name.
        /// </summary>
        /// <remarks>The enumeration is performed in a way that ensures consistent ordering across
        /// multiple calls, which can be useful for scenarios requiring predictable member order.</remarks>
        /// <returns>An enumerable collection of <see cref="ISymbol"/> representing the members of the symbol, ordered by their
        /// source span start position and then by their name.</returns>
        public IEnumerable<ISymbol> EnumerateMembersDeterministically() => self.GetMembers()
            .OrderBy(static p => p.Locations.FirstOrDefault()?.SourceSpan.Start ?? int.MaxValue)
            .ThenBy(static p => p.Name, StringComparer.Ordinal);

        /// <summary>
        /// Determines whether the current type has any partial declarations in its syntax references.
        /// </summary>
        /// <remarks>This method inspects the declaring syntax references of the type and checks for the
        /// presence of the partial modifier in any of the type declarations.</remarks>
        /// <returns>true if at least one partial declaration is found; otherwise, false.</returns>
        public bool HasPartialDeclaration() => self.DeclaringSyntaxReferences
            .Select(static reference => reference.GetSyntax())
            .OfType<TypeDeclarationSyntax>()
            .Any(static declaration => declaration.Modifiers.Any(SyntaxKind.PartialKeyword));

        /// <summary>
        /// Generates a string representation of the current object, including the fully qualified name with the global
        /// namespace.
        /// </summary>
        /// <returns>A string that represents the fully qualified name of the current object, formatted according to the
        /// specified display format.</returns>
        public string RenderGlobal() => self.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Included));

        /// <summary>
        /// Generates a fully qualified display string for a generic type, substituting the type parameters with the
        /// provided type arguments.
        /// </summary>
        /// <remarks>This method is intended for use with generic types only. If no type arguments are
        /// provided, the method returns the display string with the type parameters included.</remarks>
        /// <param name="typeArgs">The type arguments to substitute for the generic parameters of the type. Must match the number of generic
        /// parameters defined for the type.</param>
        /// <returns>A string representing the fully qualified name of the generic type with the specified type arguments
        /// applied.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the type is not a generic type or if the number of provided type arguments does not match the
        /// number of generic parameters.</exception>
        public string RenderGlobalWithGenerics(string typeArg, params string[] typeArgs)
        {
            List<string> allTypeArgs = [typeArg, ..typeArgs];
            if (!self.IsGenericType)
            {
                throw new InvalidOperationException($"Expected a generic type symbol, but got '{self.Name}' which is not generic.");
            }
            if (allTypeArgs.Count != self.TypeParameters.Length)
            {
                throw new InvalidOperationException($"The number of provided type arguments ({allTypeArgs.Count}) does not match the number of generic parameters ({self.TypeParameters.Length}) for the type '{self.Name}'.");
            }
            return $"global::{self.GetFullMetadataName().Replace($"`{self.TypeParameters.Length}", $"<{string.Join(", ", allTypeArgs)}>")}";
        }

        /// <summary>
        /// Gets the fully qualified metadata name of the type, including its namespace and any generic type parameters.
        /// </summary>
        /// <remarks>This method traverses the containing symbols of the type to construct the full
        /// metadata name, starting from the innermost type and proceeding outward to the namespace. The resulting name
        /// uniquely identifies the type within its assembly and includes generic arity information when
        /// relevant.</remarks>
        /// <returns>A string representing the complete metadata name of the type, formatted as a dot-separated list of
        /// namespaces and type names. Generic type parameters are included in the name if applicable.</returns>
        public string GetFullMetadataName(bool includeGlobalNamespacePrefix = false)
        {
            INamedTypeSymbol original = self.OriginalDefinition;

            Stack<string> parts = [];
            ISymbol? current = original;

            while (current is not null)
            {
                switch (current)
                {
                    case INamespaceSymbol ns when !ns.IsGlobalNamespace:
                        parts.Push(ns.MetadataName);
                        break;
                    case INamedTypeSymbol named:
                        parts.Push(named.MetadataName); // includes `1, `2, etc.
                        break;
                }

                current = current.ContainingSymbol;
            }
            string result = string.Join(".", parts);
            if (includeGlobalNamespacePrefix)
            {
                result = $"global::{result}";
            }
            return result;
        }
    }
}
