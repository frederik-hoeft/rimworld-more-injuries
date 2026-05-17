using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal sealed record PropertyTypeSpec(
    string PropertyDisplay,
    bool IsReferenceType,
    bool IsNullableValueType,
    bool RequiresNullCheck,
    bool FieldIsNullable)
{
    private static SymbolDisplayFormat FullyQualifiedFormat => SymbolDisplayFormats.s_fullyQualifiedWithNullable;

    public static PropertyTypeSpec Create(ITypeSymbol propertyType, DefaultValueSpec defaultValue)
    {
        bool isReferenceType = propertyType.IsReferenceType;
        bool isNullableValueType = propertyType is INamedTypeSymbol
        {
            IsValueType: true,
            OriginalDefinition.SpecialType: SpecialType.System_Nullable_T,
        };
        bool requiresNullCheck = CheckRequiresNullCheck(propertyType, defaultValue, isReferenceType, isNullableValueType);

        return new PropertyTypeSpec(
            PropertyDisplay: propertyType.ToDisplayString(FullyQualifiedFormat),
            IsReferenceType: isReferenceType,
            IsNullableValueType: isNullableValueType,
            RequiresNullCheck: requiresNullCheck,
            FieldIsNullable: CheckFieldIsNullable(propertyType, defaultValue, requiresNullCheck));
    }

    private static bool CheckRequiresNullCheck(
        ITypeSymbol propertyType,
        DefaultValueSpec defaultValue,
        bool isReferenceType,
        bool isNullableValueType)
    {
        bool isNullableAnnotated = propertyType.NullableAnnotation == NullableAnnotation.Annotated;
        bool hasNonNullDefault = defaultValue.Expression is not null && !defaultValue.IsNullable;

        return (isReferenceType && !isNullableAnnotated && !hasNonNullDefault)
            || (!isReferenceType && !isNullableValueType && defaultValue.IsNullable);
    }

    private static bool CheckFieldIsNullable(ITypeSymbol propertyType, DefaultValueSpec defaultValue, bool requiresNullCheck) =>
        requiresNullCheck
        || (propertyType.IsReferenceType && defaultValue.Expression is null)
        || (!propertyType.IsReferenceType && defaultValue.IsNullable);
}
