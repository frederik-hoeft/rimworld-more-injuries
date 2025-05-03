using RimWorld;
using Verse;
using MoreInjuries.KnownDefs;

namespace MoreInjuries.Versioning.Migrations;

internal sealed class VersionMigration2_DedicatedCprResearch : IVersionMigration
{
    private const string OK_SIGNAL = "MI_Migration_Signal_v2_Ok";

    private string? _loadID;

    public int Version => 2;

    public void ExposeData()
    {
        Scribe_Values.Look(ref _loadID, "loadID", null);
    }

    public string GetUniqueLoadID() => _loadID ??= $"MoreInjuries.VersionMigration2_DedicatedCprResearch_{Guid.NewGuid()}";

    public void Migrate()
    {
        Logger.LogDebug("VersionMigration2_DedicatedCprResearch: Migrate");
        TaggedString title = "MI_Migration_Update_v2_Title".Translate();
        TaggedString text = "MI_Migration_Update_v2_Text".Translate();

        // show changelog
        ChoiceLetter_MigrateVersion letter = (ChoiceLetter_MigrateVersion)LetterMaker.MakeLetter(title, text, KnownLetterDefOf.VersionMigration);
        letter.VersionMigration = this;
        letter.Options =
        [
            new ChoiceLetter_MigrateVersion.Option(OK_SIGNAL),
        ];
        Find.LetterStack.ReceiveLetter(letter);
    }

    public void Execute(string signal)
    {
        Logger.LogDebug($"VersionMigration2_DedicatedCprResearch: Execute: {signal}");
        if (signal is OK_SIGNAL)
        {
            UnlockCprIfWasAvailablePreviously();
        }
        else
        {
            Logger.Warning($"VersionMigration2_DedicatedCprResearch: Execute: Unknown signal '{signal}'");
        }
    }

    private void UnlockCprIfWasAvailablePreviously()
    {
        // if EmergencyMedicine is finished, unlock Cpr
        if (KnownResearchProjectDefOf.EmergencyMedicine.IsFinished)
        {
            Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.Cpr);
        }
    }
}