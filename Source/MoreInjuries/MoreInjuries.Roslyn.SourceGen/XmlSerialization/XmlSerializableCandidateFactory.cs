using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

internal static class XmlSerializableCandidateFactory
{
    public static XmlSerializableCandidate Create(XmlSerialiableTarget target)
    {
        ImmutableArray<Diagnostic>.Builder diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        INamedTypeSymbol typeSymbol = target.TypeSymbol;

        if (typeSymbol.TypeKind is not TypeKind.Class)
        {
            diagnostics.Add(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.TargetMustBeClass,
                typeSymbol.Locations.FirstOrDefault(),
                typeSymbol.Name));

            return new XmlSerializableCandidate(null, diagnostics.ToImmutable());
        }

        ImmutableArray<XmlMemberModel>.Builder memberModels = ImmutableArray.CreateBuilder<XmlMemberModel>();

        foreach (IPropertySymbol property in typeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (!property.TryGetAttribute<XmlMemberAttribute>(out AttributeData? xmlMemberAttribute)
                || xmlMemberAttribute.ConstructorArguments is not [{ Value: string fieldName }])
            {
                continue;
            }
            if (!property.IsPartialDefinition)
            {
                diagnostics.Add(Diagnostic.Create(
                    XmlSerializationGeneratorDiagnostics.MemberMustBePartial,
                    property.Locations.FirstOrDefault(),
                    property.Name,
                    typeSymbol.Name));
                continue;
            }
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                diagnostics.Add(Diagnostic.Create(
                    XmlSerializationGeneratorDiagnostics.InvalidFieldName,
                    property.Locations.FirstOrDefault(),
                    fieldName,
                    typeSymbol.Name,
                    property.Name));
                continue;
            }
            if (!typeSymbol.GetMembers(fieldName).IsEmpty)
            {
                diagnostics.Add(Diagnostic.Create(
                    XmlSerializationGeneratorDiagnostics.MemberNameConflict,
                    property.Locations.FirstOrDefault(),
                    typeSymbol.Name,
                    fieldName));
                continue;
            }
            memberModels.Add(new XmlMemberModel(property, fieldName));
        }

        string namespaceName = typeSymbol.ContainingNamespace?.IsGlobalNamespace is false
            ? typeSymbol.ContainingNamespace.ToDisplayString()
            : string.Empty;

        return new XmlSerializableCandidate(
            new XmlSerializableGenerationModel(
                Namespace: namespaceName,
                ClassSymbol: typeSymbol,
                AnnotatedMembers: memberModels.ToImmutable()),
            diagnostics.ToImmutable());
    }
}