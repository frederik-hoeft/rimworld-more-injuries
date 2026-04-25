using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.Extensions;

internal static class NamespaceSymbolExtension
{
    extension(INamespaceSymbol self)
    {
        public IEnumerable<INamedTypeSymbol> GetAllTypes()
        {
            // depth-first traversal of the namespace tree to yield all contained type symbols
            Stack<INamespaceSymbol> remaining = [];
            remaining.Push(self);
            while (remaining.Count > 0)
            {
                INamespaceSymbol namespaceSymbol = remaining.Pop();
                foreach (INamedTypeSymbol type in namespaceSymbol.GetTypeMembers())
                {
                    yield return type;
                }
                foreach (INamespaceSymbol nestedNamespace in namespaceSymbol.GetNamespaceMembers())
                {
                    remaining.Push(nestedNamespace);
                }
            }
        }
    }
}
