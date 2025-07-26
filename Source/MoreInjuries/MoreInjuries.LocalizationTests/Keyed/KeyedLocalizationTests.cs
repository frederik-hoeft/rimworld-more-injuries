using MoreInjuries.LocalizationTests.Localization;
using MoreInjuries.LocalizationTests.Model;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace MoreInjuries.LocalizationTests.Keyed;

[TestClass]
public class KeyedLocalizationTests : LocalizationTestBase
{
    [TestMethod]
    public override void LanguageFileCompletenessTest() => base.LanguageFileCompletenessTest();

    private protected override LocalizationInfoRepository LoadLocalizationInfoRepository(DirectoryInfo languageDirectory, LoadErrorContext errorContext)
    {
        Assert.IsNotNull(languageDirectory);
        LocalizationInfoRepository languageRepository = new KeyedLocalizationInfoRepository(languageDirectory.Name);
        languageRepository.Load(languageDirectory, "Keyed", errorContext);
        return languageRepository;
    }
}

internal sealed class KeyedLocalizationInfoRepository(string language) : LocalizationInfoRepository(language)
{
    protected override string CreateKey(XElement element, LocalizationInfoLoadContext context) => element.Name.LocalName;
}