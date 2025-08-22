using System.Xml.Linq;

namespace MoreInjuries.LocalizationTests.Model.Defs;

internal sealed record DefFileLoadContext(string RelativePath, FileInfo SourceFile, LoadErrorContext ErrorContext)
{
    public void ReportMissingDefName(XElement defNode) =>
        ErrorContext.Errors.Add($"[{RelativePath}]: Node {defNode.Name.LocalName} is missing a 'defName' attribute.");

    internal void ReportDuplicateKeyFor(XElement field, LocalizationValue existingValue) =>
        ErrorContext.Errors.Add($"[{RelativePath}]: Duplicate key '{field.Name.LocalName}' found in {SourceFile.FullName} for def '{existingValue.Key}'. Existing value: {existingValue.Value}.");
}