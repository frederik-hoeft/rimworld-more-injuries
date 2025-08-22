using MoreInjuries.LocalizationTests.Localization;
using MoreInjuries.LocalizationTests.Model;
using MoreInjuries.LocalizationTests.Model.Defs;

namespace MoreInjuries.LocalizationTests.DefInjected;

[TestClass]
public sealed class DefInjectedCoverageTests : LocalizationBase
{
    private protected override LocalizationInfoRepository LoadLocalizationInfoRepository(DirectoryInfo languageDirectory, LoadErrorContext errorContext)
    {
        LocalizationInfoRepository languageRepository = new DefInjectedLocalizationInfoRepository(languageDirectory.Name);
        languageRepository.Load(languageDirectory, "DefInjected", errorContext);
        return languageRepository;
    }

    [TestMethod]
    public void LocalizationCoverageTest()
    {
        LoadErrorContext errorContext = new();
        List<LocalizationInfoRepository> languageRepositories = LoadLocalizationInfoRepositories(errorContext);
        LocalizationInfoRepository? english = languageRepositories.Find(static repository => repository.Language == "English");
        Assert.IsNotNull(english, "Missing default 'English' localization data.");

        DirectoryInfo[] defsDirectories = ModRoot.GetDirectories("Defs", SearchOption.TopDirectoryOnly);
        Assert.AreEqual(1, defsDirectories.Length, "Expected exactly one 'Defs' directory in the mod root.");
        DirectoryInfo defsRoot = defsDirectories[0];
        DefDatabase defDatabase = new();
        defDatabase.Load(defsRoot, errorContext);

        foreach ((string defType, Dictionary<string, LocalizationValue> defs) in defDatabase.AllDefs)
        {
            foreach ((string defName, LocalizationValue defValue) in defs)
            {
                if (!english.LocalizationInfo.TryGetValue($"{defType}::{defName}", out LocalizationValue? englishValue))
                {
                    errorContext.Errors.Add($"[{english.Language}]: Missing translation for key '{defValue.Key}' in default 'English' localization data. This key was requested by '{defType}/{defName}'.");
                    continue;
                }
                if (!string.Equals(englishValue.Value, defValue.Value, StringComparison.Ordinal))
                {
                    errorContext.Builder.AppendLine($"[{english.Language}]: Localization mismatch for key '{defValue.Key}' in '{defType}/{defName}'.")
                        .Append(' ', english.Language.Length + 4).AppendLine($"expected (from Def): '{defValue.Value}'")
                        .Append(' ', english.Language.Length + 4).Append    ($"but found (in Lang): '{englishValue.Value}'");
                    errorContext.Errors.Add(errorContext.Builder.ToString());
                    errorContext.Builder.Clear();
                }
            }
        }
        Assert.AreEqual(0, errorContext.Errors.Count, $"Found at least one error while loading DefInjected localization data:\n{string.Join("\n", errorContext.Errors)}");
    }
}
