using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class XmlBindableMemberFactory
{
    extension (MemberAccumulator self)
    {
        public MemberAccumulator Analyze(PropertyAnalysisContext context) => self.Add(context.TryParse()
            .Bind(binding => context.ResolveGetters(binding)
            .Bind(pipeline => context.ResolveFieldDecorations(binding)
            .Bind(decorations => context.Validate(binding, self.UsedFieldNames)
            .Map(_ => new CreatedMember
            (
                binding.FieldName,
                XmlBindingModelFactory.Create(context.Property, binding, pipeline, decorations)
            ))))));
    }
}
