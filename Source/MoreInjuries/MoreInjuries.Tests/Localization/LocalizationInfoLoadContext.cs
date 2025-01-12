using System.Text;
using System.Xml.Linq;

namespace MoreInjuries.Tests.Localization;

public record LocalizationInfoLoadContext(string Language, string RelativePath, FileInfo SourceFile, LoadErrorContext ErrorContext)
{
    public void ReportMissingCommentFor(XElement element)
    {
        ErrorContext.Errors.Add($"[{Language}]: Node {element.Name.LocalName} in {RelativePath} is missing a comment.");
    }

    public void ReportInvalidCommentFor(XElement element, XNode commentNode)
    {
        ErrorContext.Builder.AppendLine($"[{Language}]: Node {element.Name.LocalName} in {RelativePath} has an invalid comment.")
            .Append(' ', Language.Length + 4).AppendLine($"expected format: '<!-- EN: <comment> -->'")
            .Append(' ', Language.Length + 4).Append($"but found:       '{commentNode}'");
        ErrorContext.Errors.Add(ErrorContext.Builder.ToString());
        ErrorContext.Builder.Clear();
    }

    public void ReportDuplicateKeyFor(LocalizationValue value1, LocalizationValue value2)
    {
        ErrorContext.Errors.Add($"[{Language}]: Duplicate key '{value1.Key}' found in {value1.Path} and {value2.Path}.");
    }
}
