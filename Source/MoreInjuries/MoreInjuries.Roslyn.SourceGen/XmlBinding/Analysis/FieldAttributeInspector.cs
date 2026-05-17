using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class FieldAttributeInspector
{
    public static bool IsConcreteFieldAttribute(INamedTypeSymbol decorateType) =>
        decorateType switch
        {
            { IsAbstract: true } => false,
            _ => InheritsFromSystemAttribute(decorateType)
                && HasPublicParameterlessConstructor(decorateType)
                && AllowsFieldTargets(decorateType),
        };

    private static bool InheritsFromSystemAttribute(INamedTypeSymbol type) =>
        type.EnumerateBaseTypes().Any(static baseType =>
            baseType.SpecialType == SpecialType.None && baseType.ToDisplayString() == "System.Attribute");

    private static bool HasPublicParameterlessConstructor(INamedTypeSymbol type) =>
        type.InstanceConstructors.Any(static constructor =>
            constructor.Parameters.IsEmpty && constructor.DeclaredAccessibility == Accessibility.Public);

    private static bool AllowsFieldTargets(INamedTypeSymbol type) =>
        type.GetAttributes()
            .Select(TryGetAttributeTargets)
            .FirstOrDefault(static targets => targets is not null) is not int targets
        || (targets & (int)AttributeTargets.Field) != 0;

    private static int? TryGetAttributeTargets(AttributeData attribute) =>
        attribute switch
        {
            {
                AttributeClass: { } attributeClass,
                ConstructorArguments: [{ Value: int targets }],
            } when attributeClass.ToDisplayString() == "System.AttributeUsageAttribute" => targets,
            _ => null,
        };
}
