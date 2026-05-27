using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class PropertyShapeValidator
{
    extension(PropertyAnalysisContext self)
    {
        public BindingResult<Unit> Validate(ParsedBindingAttribute binding, ImmutableHashSet<string> usedFieldNames) => self switch
        {
            _ when !self.Property.IsPartialDefinition =>
                self.Failure(XmlSerializationGeneratorDiagnostics.MemberMustBePartial),
            _ when string.IsNullOrWhiteSpace(binding.FieldName) || !SyntaxFacts.IsValidIdentifier(binding.FieldName) =>
                self.Failure(XmlSerializationGeneratorDiagnostics.InvalidFieldName, binding.FieldName),
            _ when !self.TypeSymbol.GetMembers(binding.FieldName).IsEmpty || usedFieldNames.Contains(binding.FieldName) =>
                self.Failure(XmlSerializationGeneratorDiagnostics.MemberNameConflict, binding.FieldName),
            _ => BindingResult<Unit>.Success(Unit.Value),
        };

        private BindingResult<Unit> Failure(DiagnosticDescriptor descriptor, params object?[] messageArgs) => BindingResult<Unit>.Failure
        (
            Diagnostic.Create(descriptor, self.Location,
            [
                self.PropertyName,
                self.TypeName,
                .. messageArgs
            ])
        );
    }
}
