using RimWorld;
using Verse;
using MoreInjuries.KnownDefs;
using MoreInjuries.Localization;

namespace MoreInjuries.Versioning.Migrations;

internal sealed class VersionMigration1_ResearchProjects : IVersionMigration
{
    private const string UNLOCK_ESSENTIAL_RESEARCH_SIGNAL = "MI_Migration_Signal_v1_UnlockEssentialResearch";
    private const string UNLOCK_ALL_RESEARCH_SIGNAL = "MI_Migration_Signal_v1_UnlockAllResearch";
    private const string REQUIRE_ALL_RESEARCH_SIGNAL = "MI_Migration_Signal_v1_RequireAllResearch";

    private string? _loadID;

    public int Version => 1;

    public void ExposeData()
    {
        Scribe_Values.Look(ref _loadID, "loadID", null);
    }

    public string GetUniqueLoadID() => _loadID ??= $"MoreInjuries.VersionMigration1_ResearchProjects_{Guid.NewGuid()}";

    public void Migrate()
    {
        Logger.LogDebug("VersionMigration1_ResearchProjects: Migrate");
        Pawn? randomColonist = Find.CurrentMap?.mapPawns?.FreeColonists?.MaxBy(pawn => pawn.skills?.GetSkill(SkillDefOf.Medicine)?.Level ?? -1);
        TaggedString title = "MI_Migration_Update_v1_Title".Translate();
        TaggedString text;
        if (randomColonist is not null)
        {
            text = "MI_Migration_Update_v1_Text_Personalized".Translate(randomColonist.Named(Named.Params.PAWN));
        }
        else
        {
            text = "MI_Migration_Update_v1_Text".Translate();
        }
        // show changelog + ask whether to auto-complete research for existing saves
        ChoiceLetter_MigrateVersion letter = (ChoiceLetter_MigrateVersion)LetterMaker.MakeLetter(title, text, KnownLetterDefOf.VersionMigration);
        letter.VersionMigration = this;
        letter.Options =
        [
            new ChoiceLetter_MigrateVersion.Option(REQUIRE_ALL_RESEARCH_SIGNAL),
            new ChoiceLetter_MigrateVersion.Option(UNLOCK_ESSENTIAL_RESEARCH_SIGNAL),
            new ChoiceLetter_MigrateVersion.Option(UNLOCK_ALL_RESEARCH_SIGNAL)
        ];
        Find.LetterStack.ReceiveLetter(letter);
    }

    public void Execute(string signal)
    {
        Logger.LogDebug($"VersionMigration1_ResearchProjects: Execute: {signal}");
        if (signal is REQUIRE_ALL_RESEARCH_SIGNAL)
        {
            EnableResearchProjects();
        }
        else if (signal is UNLOCK_ALL_RESEARCH_SIGNAL)
        {
            UnlockAllResearchProjects();
        }
        else if (signal is UNLOCK_ESSENTIAL_RESEARCH_SIGNAL)
        {
            UnlockEssentialResearchProjects();
        }
        else
        {
            Logger.Warning($"VersionMigration1_ResearchProjects: Execute: Unknown signal '{signal}'");
        }
    }

    private void EnableResearchProjects() { }

    private void UnlockEssentialResearchProjects()
    {
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.BasicAnatomy);
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.BasicFirstAid);
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.EmergencyMedicine);
    }

    private void UnlockAllResearchProjects()
    {
        UnlockEssentialResearchProjects();
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.AdvancedFirstAid);
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.AdvancedThoracicSurgery);
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.CellularRegeneration);
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.EpinephrineSynthesis);
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.Neurosurgery);
    }
}