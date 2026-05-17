using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class GetterPipelineResolver
{
    public static BindingResult<GetterPipelineSpec> Resolve(
        AttributeData attribute,
        PropertyAnalysisContext context) =>
        ResolveOptionalMethod(
            attribute,
            nameof(XmlBindingAttribute.Validate),
            methodName => GetterMethodSymbolResolver.ResolveValidateMethod(context, methodName))
        .Bind(validate => ResolveOptionalMethod(
            attribute,
            nameof(XmlBindingAttribute.Transform),
            methodName => GetterMethodSymbolResolver.ResolveTransformMethod(context, methodName))
        .Map(transform => new GetterPipelineSpec(validate, transform)));

    private static BindingResult<MethodCallModel?> ResolveOptionalMethod(
        AttributeData attribute,
        string attributeMemberName,
        Func<string, BindingResult<ResolvedMethod>> resolve) =>
        attribute.GetNamedStringArgument(attributeMemberName) switch
        {
            { } methodName => resolve(methodName)
                .Map<MethodCallModel?>(static method => new MethodCallModel(method.MethodName, method.IsStatic)),
            null => BindingResult<MethodCallModel?>.Success(null),
        };
}
