using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using MoreInjuries.Tests.Localization;

namespace MoreInjuries.Tests;

public abstract class LocalizationTestBase
{
    protected abstract LocalizationInfoRepository LoadLocalizationInfoRepository(DirectoryInfo languageDirectory, LoadErrorContext errorContext);

    protected static DirectoryInfo GetModRoot()
    {
        string assemblyLocation = typeof(KeyedLocalizationTests).Assembly.GetAssemblyLocation();
        FileInfo assemblyFile = new(assemblyLocation);
        // /Source/MoreInjuries/MoreInjuries.Tests/bin/*/net472/MoreInjuries.Tests.dll
        if (assemblyFile is not { Directory.Parent.Parent.Parent.Parent.Parent.Parent: DirectoryInfo modRoot })
        {
            throw new InvalidOperationException("Failed to locate the mod root directory.");
        }
        Assert.AreEqual(1, modRoot.GetDirectories(".git", SearchOption.TopDirectoryOnly).Length, "Suspected mod root directory is not a git repository.");
        return modRoot;
    }

    public virtual void LanguageFileCompletenessTest()
    {
        DirectoryInfo modRoot = GetModRoot();
        DirectoryInfo[] languageDirectories = modRoot.GetDirectories("Languages", SearchOption.TopDirectoryOnly);
        Assert.AreEqual(1, languageDirectories.Length, "Expected exactly one 'Languages' directory in the mod root.");
        DirectoryInfo localizationRoot = languageDirectories[0];
        List<LocalizationInfoRepository> languageRepositories = [];
        LoadErrorContext errorContext = new();
        foreach (DirectoryInfo languageDirectory in localizationRoot.EnumerateDirectories())
        {
            LocalizationInfoRepository languageRepository = LoadLocalizationInfoRepository(languageDirectory, errorContext);
            languageRepositories.Add(languageRepository);
        }
        LocalizationInfoRepository? english = languageRepositories.Find(static repository => repository.Language == "English");
        Assert.IsNotNull(english, "Missing default 'English' localization data.");
        foreach (LocalizationInfoRepository repository in languageRepositories)
        {
            foreach (LocalizationValue value in repository.LocalizationInfo.Values)
            {
                // all keys must be defined in the default 'English' localization data
                if (!english.LocalizationInfo.TryGetValue(value.Key, out LocalizationValue? englishValue))
                {
                    errorContext.Errors.Add($"[{repository.Language}]: Invalid key '{value.Key}' found. Key was never defined in default 'English' localization data.");
                    continue;
                }
                // all keys must have the most up-to-date comment for the 'English version' of the key
                if (!string.Equals(englishValue.Value, value.Comment, StringComparison.Ordinal))
                {
                    errorContext.Builder.AppendLine($"[{repository.Language}]: Outdated comment for key '{value.Key}' in {value.Path}.")
                        .Append(' ', repository.Language.Length + 4).AppendLine($"expected:  '{englishValue.Value}'")
                        .Append(' ', repository.Language.Length + 4).Append($"but found: '{value.Comment}'");
                    errorContext.Errors.Add(errorContext.Builder.ToString());
                    errorContext.Builder.Clear();
                }
                // the file path of the entry must match the file path of the 'English' version of the key
                if (!string.Equals(englishValue.Path, value.Path, StringComparison.Ordinal))
                {
                    errorContext.Builder.AppendLine($"[{repository.Language}]: File path mismatch for key '{value.Key}'.")
                        .Append(' ', repository.Language.Length + 4).AppendLine($"expected:  '{englishValue.Path}'")
                        .Append(' ', repository.Language.Length + 4).Append($"but found: '{value.Path}'");
                    errorContext.Errors.Add(errorContext.Builder.ToString());
                    errorContext.Builder.Clear();
                }
            }
        }
        // all keys in the 'English' localization data must be defined in all other localization data
        foreach (LocalizationValue value in english.LocalizationInfo.Values)
        {
            foreach (LocalizationInfoRepository repository in languageRepositories)
            {
                if (!repository.LocalizationInfo.ContainsKey(value.Key))
                {
                    errorContext.Errors.Add($"[{repository.Language}]: Missing localization data for key '{value.Key}'. Expected to find key in '{value.Path}'.");
                }
            }
        }
        Assert.AreEqual(0, errorContext.Errors.Count, $"Found at least one error while loading localization data:\n{string.Join("\n", errorContext.Errors)}");
    }
}
