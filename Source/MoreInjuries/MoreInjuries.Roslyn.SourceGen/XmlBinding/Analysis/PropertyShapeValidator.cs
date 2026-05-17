using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class PropertyShapeValidator
{
    public static BindingResult<Unit> Validate(
        PropertyAnalysisContext context,
        string fieldName,
        ImmutableHashSet<string> usedFieldNames) =>
        context.Property switch
        {
            _ when !context.Property.IsPartialDefinition => Failure(
                XmlSerializationGeneratorDiagnostics.MemberMustBePartial,
                context),
            _ when string.IsNullOrWhiteSpace(fieldName) || !SyntaxFacts.IsValidIdentifier(fieldName) => Failure(
                XmlSerializationGeneratorDiagnostics.InvalidFieldName,
                context,
                fieldName),
            _ when !context.TypeSymbol.GetMembers(fieldName).IsEmpty || usedFieldNames.Contains(fieldName) => Failure(
                XmlSerializationGeneratorDiagnostics.MemberNameConflict,
                context,
                fieldName),
            _ => BindingResult<Unit>.Success(Unit.Value),
        };

    private static BindingResult<Unit> Failure(
        DiagnosticDescriptor descriptor,
        PropertyAnalysisContext context,
        params object?[] messageArgs)
    {
        object?[] diagnosticArgs = [context.PropertyName, context.TypeName, .. messageArgs];
        return BindingResult<Unit>.Failure(Diagnostic.Create(
            descriptor,
            context.Location,
            diagnosticArgs));
    }
}
