using MoreInjuries.LocalizationTests.Model;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace MoreInjuries.LocalizationTests.Localization;

internal abstract partial class LocalizationInfoRepository(string language)
{
    private bool _isLoaded = false;

    public string Language { get; } = language;

    [GeneratedRegex(@"^<!-- EN: (?<comment>.+) -->$")]
    private static partial Regex CommentNodeRegex { get; }

    [GeneratedRegex(@"^<!-- OPTS: (?<options>{\s*(""(?<key_1>[A-Za-z][A-Za-z0-9_\-]*)"":\s*(?<value_1>(true|false)))(,\s*""(?<key_n>[A-Za-z][A-Za-z0-9_\-]*)"":\s*(?<value_n>(true|false)))*\s*}) -->$")]
    private static partial Regex OptionsNodeRegex { get; }

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
            string relativePath = file.FullName[parentPathLength..];
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

    private protected virtual Dictionary<string, LocalizationValue> LoadLocalizationScope(FileInfo file, LocalizationInfoLoadContext context)
    {
        Dictionary<string, LocalizationValue> keyedLocalizationInfo = [];
        using FileStream stream = file.OpenRead();
        XDocument document = XDocument.Load(stream);
        foreach (XElement element in document.Root?.Elements() ?? [])
        {
            List<XNode> commentNodes = [];
            for (XNode? node = element.PreviousNode; node is { NodeType: XmlNodeType.Comment }; node = node.PreviousNode)
            {
                commentNodes.Add(node);
            }
            string? comment = null;
            IReadOnlyDictionary<string, bool> nodeOptions = Options.Empty;
            foreach (XNode commentNode in commentNodes)
            {
                string commentNodeString = commentNode.ToString();
                if (CommentNodeRegex.Match(commentNodeString) is { Success: true } commentMatch)
                {
                    Group commentGroup = commentMatch.Groups["comment"];
                    comment = commentGroup.Value;
                    continue;
                }
                if (OptionsNodeRegex.Matches(commentNodeString) is { Count: > 0 } optionsMatches)
                {
                    Match optionsMatch = optionsMatches[0];
                    GroupCollection groups = optionsMatch.Groups;
                    Dictionary<string, bool> options = [];
                    // key_1 and value_1 are always present if there is a match
                    options[groups["key_1"].Value] = bool.Parse(groups["value_1"].Value);
                    for (int i = 0; i < groups["key_n"].Captures.Count; ++i)
                    {
                        options[groups["key_n"].Captures[i].Value] = bool.Parse(groups["value_n"].Captures[i].Value);
                    }
                    nodeOptions = options;
                    continue;
                }
            }
            if (string.IsNullOrEmpty(comment))
            {
                context.ReportMissingCommentFor(element);
            }
            string key = CreateKey(element, context);
            LocalizationValue localizationValue = new(key, context.RelativePath, element.Value, comment, nodeOptions);
            keyedLocalizationInfo[localizationValue.Key] = localizationValue;
        }
        return keyedLocalizationInfo;
    }

    protected abstract string CreateKey(XElement element, LocalizationInfoLoadContext context);
}
