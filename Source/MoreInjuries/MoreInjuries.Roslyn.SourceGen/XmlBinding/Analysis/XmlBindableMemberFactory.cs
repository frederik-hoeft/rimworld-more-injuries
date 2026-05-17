using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class XmlBindableMemberFactory
{
    public static BindingResult<CreatedMember>? TryCreate(PropertyAnalysisContext context, ImmutableHashSet<string> usedFieldNames) => context.TryParse()?
        .Bind(binding => context.ResolveGetters(binding)
        .Bind(pipeline => context.ResolveFieldDecorations(binding)
        .Bind(decorations => context.Validate(binding, usedFieldNames)
        .Map(_ => new CreatedMember
        (
            binding.FieldName,
            XmlBindingModelFactory.Create(context.Property, binding, pipeline, decorations)
        )))));
}
