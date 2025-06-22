using MoreInjuries.LocalizationTests.Localization;
using System.Xml.Linq;

namespace MoreInjuries.LocalizationTests;

[TestClass]
public class KeyedLocalizationTests : LocalizationTestBase
{
    [TestMethod]
    public override void LanguageFileCompletenessTest() => base.LanguageFileCompletenessTest();

    protected override LocalizationInfoRepository LoadLocalizationInfoRepository(DirectoryInfo languageDirectory, LoadErrorContext errorContext)
    {
        LocalizationInfoRepository languageRepository = new KeyedLocalizationInfoRepository(languageDirectory.Name);
        languageRepository.Load(languageDirectory, "Keyed", errorContext);
        return languageRepository;
    }
}

internal class KeyedLocalizationInfoRepository(string language) : LocalizationInfoRepository(language)
{
    protected override string CreateKey(XElement element, LocalizationInfoLoadContext context) => element.Name.LocalName;
}