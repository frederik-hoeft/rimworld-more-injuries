using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

internal sealed record ConcreteCollectionMapping(string Display, INamedTypeSymbol? Symbol);
