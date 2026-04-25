using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.Extensions;

internal static class TypeSymbolExtensions
{
    extension (ITypeSymbol self)
    {
        /// <summary>
        /// Attempts to unwrap a nullable type, returning the underlying type if successful.
        /// </summary>
        /// <remarks>This method checks if the current type is annotated as nullable or if it is a
        /// nullable type. If so, it provides the underlying type without the nullable annotation.</remarks>
        /// <param name="unwrapped">When the method returns <see langword="true"/>, contains the unwrapped underlying type symbol. Otherwise,
        /// contains the original type symbol.</param>
        /// <returns>true if the type was successfully unwrapped; otherwise, false.</returns>
        public bool TryUnwrapNullableType(out ITypeSymbol unwrapped)
        {
            if (self.NullableAnnotation == NullableAnnotation.Annotated)
            {
                unwrapped = self.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
                return true;
            }
            if (self is INamedTypeSymbol namedType && namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            {
                unwrapped = namedType.TypeArguments[0];
                return true;
            }
            unwrapped = self;
            return false;
        }
    }
}
