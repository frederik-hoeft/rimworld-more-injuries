using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class GetterPipelineResolver
{
    extension(PropertyAnalysisContext self)
    {
        public BindingResult<GetterPipelineSpec> ResolveGetters(ParsedBindingAttribute binding) => self
            .ResolveOptionalMethod(binding, nameof(XmlBindingAttribute.Validate),
                methodName => GetterMethodSymbolResolver.ResolveValidateMethod(self, methodName))
            .Bind(validate => self
                .ResolveOptionalMethod(binding, nameof(XmlBindingAttribute.Transform), methodName =>
                    GetterMethodSymbolResolver.ResolveTransformMethod(self, methodName))
                .Map(transform => new GetterPipelineSpec(validate, transform)));

        private BindingResult<MethodCallModel?> ResolveOptionalMethod(ParsedBindingAttribute binding, string attributeMemberName, Func<string, BindingResult<ResolvedMethod>> resolve) =>
            binding.Attribute.GetNamedStringArgument(attributeMemberName) is { } methodName
                ? resolve(methodName).Map<MethodCallModel?>(static method => new MethodCallModel(method.MethodName, method.IsStatic))
                : BindingResult<MethodCallModel?>.Success(null);
    }
}
