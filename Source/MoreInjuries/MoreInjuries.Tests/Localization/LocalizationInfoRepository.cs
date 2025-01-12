using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace MoreInjuries.Tests.Localization;

public abstract class LocalizationInfoRepository(string language)
{
    private static readonly Regex s_commentNodeRegex = new(@"^<!-- EN: (?<comment>.+) -->$", RegexOptions.Compiled);

    private bool _isLoaded = false;

    public string Language { get; } = language;

    public Dictionary<string, Dictionary<string, LocalizationValue>> ScopedLocalizationInfo { get; } = [];

    public Dictionary<string, LocalizationValue> LocalizationInfo { get; } = [];

    public virtual void Load(DirectoryInfo languageRoot, string child, LoadErrorContext errorContext)
    {
        Assert.IsFalse(_isLoaded, $"Localization data for '{Language}' has already been loaded.");
        _isLoaded = true;

        DirectoryInfo? keyedLocalizationDirectory = languageRoot.EnumerateDirectories(child, SearchOption.TopDirectoryOnly).FirstOrDefault();
        Assert.IsNotNull(keyedLocalizationDirectory, $"Expected a '{child}' directory in the '{Language}' language directory.");

        int parentPathLength = keyedLocalizationDirectory.FullName.Length + 1;
        foreach (FileInfo file in keyedLocalizationDirectory.EnumerateFiles("*.xml", SearchOption.AllDirectories))
        {
            string relativePath = file.FullName.Substring(parentPathLength);
            LocalizationInfoLoadContext context = new(Language, relativePath, file, errorContext);
            Dictionary<string, LocalizationValue> localizationScope = LoadLocalizationScope(file, context);
            foreach (LocalizationValue localizationValue in localizationScope.Values)
            {
                if (LocalizationInfo.TryGetValue(localizationValue.Key, out LocalizationValue? existingValue))
                {
                    context.ReportDuplicateKeyFor(localizationValue, existingValue);
                    continue;
                }
                LocalizationInfo[localizationValue.Key] = localizationValue;
            }
            ScopedLocalizationInfo[relativePath] = localizationScope;
        }
    }

    protected virtual Dictionary<string, LocalizationValue> LoadLocalizationScope(FileInfo file, LocalizationInfoLoadContext context)
    {
        Dictionary<string, LocalizationValue> keyedLocalizationInfo = [];
        using FileStream stream = file.OpenRead();
        XDocument document = XDocument.Load(stream);
        foreach (XElement element in document.Root.Elements())
        {
            XNode? commentNode = element.PreviousNode;
            string? comment = null;
            if (commentNode is { NodeType: XmlNodeType.Comment })
            {
                Match match = s_commentNodeRegex.Match(commentNode.ToString());
                if (match.Success)
                {
                    Group commentGroup = match.Groups["comment"];
                    comment = commentGroup.Value;
                }
                else
                {
                    context.ReportInvalidCommentFor(element, commentNode);
                }
            }
            else
            {
                context.ReportMissingCommentFor(element);
            }
            string key = CreateKey(element, context);
            LocalizationValue localizationValue = new(key, context.RelativePath, element.Value, comment);
            keyedLocalizationInfo[localizationValue.Key] = localizationValue;
        }
        return keyedLocalizationInfo;
    }

    protected abstract string CreateKey(XElement element, LocalizationInfoLoadContext context);
}
