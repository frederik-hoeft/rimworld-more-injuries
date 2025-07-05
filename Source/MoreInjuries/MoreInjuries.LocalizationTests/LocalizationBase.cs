using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using MoreInjuries.LocalizationTests.Keyed;
using MoreInjuries.LocalizationTests.Localization;
using MoreInjuries.LocalizationTests.Model;

namespace MoreInjuries.LocalizationTests;

public abstract class LocalizationBase
{
    private protected static DirectoryInfo ModRoot { get; } = GetModRoot();

    private protected static DirectoryInfo GetModRoot()
    {
        string assemblyLocation = typeof(KeyedLocalizationTests).Assembly.GetAssemblyLocation();
        FileInfo assemblyFile = new(assemblyLocation);
        // /Source/MoreInjuries/MoreInjuries.Tests/bin/*/net9.0/MoreInjuries.Tests.dll
        if (assemblyFile is not { Directory.Parent.Parent.Parent.Parent.Parent.Parent: DirectoryInfo modRoot })
        {
            throw new InvalidOperationException("Failed to locate the mod root directory.");
        }
        Assert.AreEqual(1, modRoot.GetDirectories(".git", SearchOption.TopDirectoryOnly).Length, "Suspected mod root directory is not a git repository.");
        return modRoot;
    }

    private protected List<LocalizationInfoRepository> LoadLocalizationInfoRepositories(LoadErrorContext errorContext)
    {
        DirectoryInfo modRoot = ModRoot;
        DirectoryInfo[] languageDirectories = modRoot.GetDirectories("Languages", SearchOption.TopDirectoryOnly);
        Assert.AreEqual(1, languageDirectories.Length, "Expected exactly one 'Languages' directory in the mod root.");
        DirectoryInfo localizationRoot = languageDirectories[0];
        List<LocalizationInfoRepository> languageRepositories = [];
        foreach (DirectoryInfo languageDirectory in localizationRoot.EnumerateDirectories())
        {
            LocalizationInfoRepository languageRepository = LoadLocalizationInfoRepository(languageDirectory, errorContext);
            languageRepositories.Add(languageRepository);
        }
        return languageRepositories;
    }

    private protected abstract LocalizationInfoRepository LoadLocalizationInfoRepository(DirectoryInfo languageDirectory, LoadErrorContext errorContext);
}