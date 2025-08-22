using MoreInjuries.LocalizationTests.Localization;
using MoreInjuries.LocalizationTests.Model;
using System.Xml.Linq;

namespace MoreInjuries.LocalizationTests.DefInjected;

[TestClass]
public sealed class DefInjectedLocalizationTests : LocalizationTestBase
{
    private protected override LocalizationInfoRepository LoadLocalizationInfoRepository(DirectoryInfo languageDirectory, LoadErrorContext errorContext)
    {
        LocalizationInfoRepository languageRepository = new DefInjectedLocalizationInfoRepository(languageDirectory.Name);
        languageRepository.Load(languageDirectory, "DefInjected", errorContext);
        return languageRepository;
    }

    [TestMethod]
    public override void LanguageFileCompletenessTest() => base.LanguageFileCompletenessTest();
}

internal sealed class DefInjectedLocalizationInfoRepository(string language) : LocalizationInfoRepository(language)
{
    protected override string CreateKey(XElement element, LocalizationInfoLoadContext context) => $"{context.SourceFile.Directory?.Name}::{element.Name.LocalName}";
}
