using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class XmlBindableMemberFactory
{
    public static BindingResult<CreatedMember>? TryCreate(
        PropertyAnalysisContext context,
        ImmutableHashSet<string> usedFieldNames) =>
        XmlBindingAttributeParser.TryParse(context)?.Bind(parsed =>
            GetterPipelineResolver.Resolve(parsed.Attribute, context).Bind(pipeline =>
            FieldDecorationResolver.Resolve(parsed.Attribute, context).Bind(decorations =>
            PropertyShapeValidator.Validate(context, parsed.FieldName, usedFieldNames).Map(_ =>
                new CreatedMember(
                    parsed.FieldName,
                    XmlBindingModelFactory.Create(context.Property, parsed, pipeline, decorations))))));
}
