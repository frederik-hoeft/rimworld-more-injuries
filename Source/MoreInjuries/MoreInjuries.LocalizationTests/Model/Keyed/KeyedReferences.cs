using System.Text.RegularExpressions;

namespace MoreInjuries.LocalizationTests.Model.Keyed;

internal sealed partial class KeyedReferences
{
    public Dictionary<string, List<KeyedReference>> References { get; } = [];

    [GeneratedRegex(@"^((?:(?<!//).)*?""(?<translation_key>MI_[A-Za-z0-9_]+))*""")]
    private static partial Regex LocalizationStringRegex { get; }

    public void Load(DirectoryInfo directory, LoadErrorContext errorContext)
    {
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(errorContext);
        FileInfo[] sourceFiles = directory.GetFiles("*.cs", SearchOption.AllDirectories);
        foreach (FileInfo file in sourceFiles)
        {
            Load(directory, file, errorContext);
        }
    }

    public void Load(DirectoryInfo rootDirectory, FileInfo file, LoadErrorContext errorContext)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(errorContext);
        int lineNumber = 0;
        using StreamReader reader = file.OpenText();
        ReadOnlySpan<char> fileNameSpan = file.FullName.AsSpan(rootDirectory.FullName.Length + 1);
        Span<char> fileNameSpanBuffer = stackalloc char[fileNameSpan.Length];
        fileNameSpan.Replace(fileNameSpanBuffer, '\\', '/');
        string normalizedFileName = new(fileNameSpanBuffer);
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine() ?? string.Empty;
            ++lineNumber;
            MatchCollection matches = LocalizationStringRegex.Matches(line);
            foreach (Match match in matches)
            {
                Group group = match.Groups["translation_key"];
                if (!group.Success)
                {
                    continue;
                }
                string key = group.Value;
                if (!References.TryGetValue(key, out List<KeyedReference>? references))
                {
                    references = [];
                    References[key] = references;
                }
                
                references.Add(new KeyedReference(key, normalizedFileName, lineNumber));
            }
        }
    }
}
