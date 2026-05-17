using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class DefaultValueResolver
{
    private static SymbolDisplayFormat FullyQualifiedFormat => SymbolDisplayFormats.s_fullyQualifiedWithNullable;

    public static BindingResult<DefaultValueSpec> Resolve(
        AttributeData xmlMemberAttribute,
        PropertyAnalysisContext context)
    {
        INamedTypeSymbol? defaultValueProvider = xmlMemberAttribute.GetNamedTypeArgument(nameof(XmlBindingAttribute.DefaultValueProvider));
        string? defaultValueFrom = xmlMemberAttribute.GetNamedStringArgument(nameof(XmlBindingAttribute.DefaultValueFrom));

        if (defaultValueProvider is not null && defaultValueFrom is null)
        {
            return BindingResult<DefaultValueSpec>.Failure(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.DefaultValueProviderRequiresDefaultValueFrom,
                context.Location,
                context.PropertyName,
                context.TypeName));
        }

        return ResolveSource(
            provider: defaultValueProvider,
            context: new DefaultValueSourceContext(context, GetBackingFieldTargetType(context.Property), defaultValueFrom ?? string.Empty));
    }

    private static BindingResult<DefaultValueSpec> ResolveSource(
        INamedTypeSymbol? provider,
        DefaultValueSourceContext context) =>
        (provider, context.SourceName) switch
        {
            ({ } providerType, { Length: > 0 }) => DefaultValueSymbolResolver
                .ResolveFromProvider(providerType, context)
                .Map(ToDefaultValueSpec),
            (null, { Length: > 0 }) => DefaultValueSymbolResolver
                .ResolveFromDeclaringType(context)
                .Map(ToDefaultValueSpec),
            _ => BindingResult<DefaultValueSpec>.Success(new DefaultValueSpec(Expression: null, IsNullable: false)),
        };

    private static DefaultValueSpec ToDefaultValueSpec(DefaultValueResolution resolution) =>
        new(resolution.Expression, resolution.IsNullable);

    private static ITypeSymbol GetBackingFieldTargetType(IPropertySymbol property)
    {
        CollectionTypeMapper.TryGetConcreteType(property.Type, FullyQualifiedFormat, out INamedTypeSymbol? concreteTypeSymbol);
        return concreteTypeSymbol ?? property.Type;
    }
}
