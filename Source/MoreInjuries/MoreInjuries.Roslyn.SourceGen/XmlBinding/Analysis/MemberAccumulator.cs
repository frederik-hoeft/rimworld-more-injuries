using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal sealed record MemberAccumulator
(
    ImmutableList<XmlBindingModel> Members,
    ImmutableList<Diagnostic> Diagnostics,
    ImmutableHashSet<string> UsedFieldNames
)
{
    public static MemberAccumulator Empty { get; } = new(Members: [], Diagnostics: [], UsedFieldNames: ImmutableHashSet<string>.Empty.WithComparer(StringComparer.Ordinal));

    public MemberAccumulator Add(BindingResult<CreatedMember>? result) => result switch
    {
        null => this,
        { IsSuccess: true, Value: { } created } => this with
        {
            Members = Members.Add(created.Model),
            UsedFieldNames = UsedFieldNames.Add(created.FieldName),
        },
        { } failed => this with
        {
            Diagnostics = Diagnostics.AddRange(failed.Diagnostics),
        },
    };
}
