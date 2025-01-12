using MoreInjuries.Tests.Localization;
using System.Text;
using System.Xml.Linq;

namespace MoreInjuries.Tests;

[TestClass]
public class DefInjectedLocalizationTests : LocalizationTestBase
{
    protected override LocalizationInfoRepository LoadLocalizationInfoRepository(DirectoryInfo languageDirectory, LoadErrorContext errorContext)
    {
        LocalizationInfoRepository languageRepository = new DefInjectedLocalizationInfoRepository(languageDirectory.Name);
        languageRepository.Load(languageDirectory, "DefInjected", errorContext);
        return languageRepository;
    }

    [TestMethod]
    public override void LanguageFileCompletenessTest() => base.LanguageFileCompletenessTest();
}

internal class DefInjectedLocalizationInfoRepository(string language) : LocalizationInfoRepository(language)
{
    protected override string CreateKey(XElement element, LocalizationInfoLoadContext context) => $"{context.SourceFile.Directory.Name}::{element.Name.LocalName}";
}
