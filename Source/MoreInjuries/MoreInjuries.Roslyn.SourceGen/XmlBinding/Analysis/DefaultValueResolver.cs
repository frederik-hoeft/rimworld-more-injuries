using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class DefaultValueResolver
{
    private static SymbolDisplayFormat FullyQualifiedFormat => SymbolDisplayFormats.s_fullyQualifiedWithNullable;

    public static BindingResult<DefaultValueSpec> Resolve(AttributeData xmlMemberAttribute, PropertyAnalysisContext context)
    {
        INamedTypeSymbol? defaultValueProvider = xmlMemberAttribute.GetNamedTypeArgument(nameof(XmlBindingAttribute.DefaultValueProvider));
        string? defaultValueFrom = xmlMemberAttribute.GetNamedStringArgument(nameof(XmlBindingAttribute.DefaultValueFrom));

        if (defaultValueFrom is null)
        {
            if (defaultValueFrom is null)
            {
                return BindingResult<DefaultValueSpec>.Success(new DefaultValueSpec(Expression: null, IsNullable: false));
            }
            return BindingResult<DefaultValueSpec>.Failure(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.DefaultValueProviderRequiresDefaultValueFrom,
                context.Location,
                context.PropertyName,
                context.TypeName));
        }

        return ResolveSource(defaultValueProvider, new DefaultValueSourceContext
        (
            context,
            GetBackingFieldTargetType(context.Property),
            defaultValueFrom
        ));
    }

    private static BindingResult<DefaultValueSpec> ResolveSource(INamedTypeSymbol? provider, DefaultValueSourceContext context) => provider switch
    {
        { } providerType => DefaultValueSymbolResolver
            .ResolveFromProvider(providerType, context)
            .Map(ToDefaultValueSpec),
        _ => DefaultValueSymbolResolver
            .ResolveFromDeclaringType(context)
            .Map(ToDefaultValueSpec)
    };

    private static DefaultValueSpec ToDefaultValueSpec(DefaultValueResolution resolution) => new(resolution.Expression, resolution.IsNullable);

    private static ITypeSymbol GetBackingFieldTargetType(IPropertySymbol property)
    {
        CollectionTypeMapper.TryGetConcreteType(property.Type, FullyQualifiedFormat, out INamedTypeSymbol? concreteTypeSymbol);
        return concreteTypeSymbol ?? property.Type;
    }
}
