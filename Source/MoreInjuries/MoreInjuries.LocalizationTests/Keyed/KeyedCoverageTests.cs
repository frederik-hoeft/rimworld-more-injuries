using MoreInjuries.LocalizationTests.Localization;
using MoreInjuries.LocalizationTests.Model;
using MoreInjuries.LocalizationTests.Model.Keyed;

namespace MoreInjuries.LocalizationTests.Keyed;

[TestClass]
public sealed class KeyedCoverageTests : LocalizationBase
{
    private protected override LocalizationInfoRepository LoadLocalizationInfoRepository(DirectoryInfo languageDirectory, LoadErrorContext errorContext)
    {
        Assert.IsNotNull(languageDirectory);
        LocalizationInfoRepository languageRepository = new KeyedLocalizationInfoRepository(languageDirectory.Name);
        languageRepository.Load(languageDirectory, "Keyed", errorContext);
        return languageRepository;
    }

    [TestMethod]
    public void LocalizationCoverageTest()
    {
        LoadErrorContext errorContext = new();
        List<LocalizationInfoRepository> languageRepositories = LoadLocalizationInfoRepositories(errorContext);
        LocalizationInfoRepository? english = languageRepositories.Find(static repository => repository.Language == "English");
        Assert.IsNotNull(english, "Missing default 'English' localization data.");

        string path = Path.Combine(ModRoot.FullName, "Source", "MoreInjuries", "MoreInjuries");
        DirectoryInfo sourceRoot = new(path);
        Assert.IsTrue(sourceRoot.Exists, $"Expected the source directory to exist at '{sourceRoot.FullName}'.");
        KeyedReferences keyedReferences = new();
        keyedReferences.Load(sourceRoot, errorContext);

        HashSet<string> visitedKeys = new(capacity: keyedReferences.References.Count);
        foreach ((string localizationKey, List<KeyedReference> references) in keyedReferences.References)
        {
            Assert.IsTrue(visitedKeys.Add(localizationKey), $"Duplicate localization key '{localizationKey}' found in source code references. This should be impossible.");
            if (!english.LocalizationInfo.ContainsKey(localizationKey))
            {
                errorContext.Builder.AppendLine($"[{english.Language}]: Missing translation for key '{localizationKey}' in default 'English' localization data. This key was requested by:");
                foreach (KeyedReference reference in references)
                {
                    errorContext.Builder.Append(' ', english.Language.Length + 4).Append($"- {reference.SourceFile} (line {reference.LineNumber})");
                }
                errorContext.Errors.Add(errorContext.Builder.ToString());
                errorContext.Builder.Clear();
            }
        }
        foreach (LocalizationValue value in english.LocalizationInfo.Values)
        {
            if (!visitedKeys.Contains(value.Key) && !(value.Options.TryGetValue("allow_unused", out bool allowUnused) && allowUnused))
            {
                errorContext.Errors.Add($"[{english.Language}]: Unused translation for key '{value.Key}' found in default 'English' localization data. This key is not referenced anywhere in the source code.");
            }
        }
        Assert.IsEmpty(errorContext.Errors, $"Found at least one error while loading DefInjected localization data:\n{string.Join("\n", errorContext.Errors)}");
    }
}
