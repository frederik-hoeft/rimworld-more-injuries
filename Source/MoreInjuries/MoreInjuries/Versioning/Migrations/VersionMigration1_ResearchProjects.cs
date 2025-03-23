using RimWorld;
using Verse;
using MoreInjuries.KnownDefs;

namespace MoreInjuries.Versioning.Migrations;

internal class VersionMigration1_ResearchProjects : IVersionMigration
{
    public int Version => 1;

    public void Migrate()
    {
        // show changelog + ask whether to auto-complete research for existing saves
        Logger.LogDebug("VersionMigration1_ResearchProjects: Migrate");
        TaggedString title = "MI_Migration_Update_v1_Title".Translate();
        TaggedString text = "MI_Migration_Update_v1_Text".Translate();
        ChoiceLetter_MigrateVersion letter = (ChoiceLetter_MigrateVersion)LetterMaker.MakeLetter(title, text, KnownLetterDefOf.VersionMigration);
        letter.Options =
        [
            new ChoiceLetter_MigrateVersion.Option("MI_Migration_Update_v1_NewBehavior".Translate(), EnableResearchProjects),
            new ChoiceLetter_MigrateVersion.Option("MI_Migration_Update_v1_OldBehavior".Translate(), UnlockAllResearchProjects)
        ];
        Find.LetterStack.ReceiveLetter(letter);
    }

    private void EnableResearchProjects() { }

    private void UnlockAllResearchProjects()
    {
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.AdvancedFirstAid);
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.AdvancedThoracicSurgery);
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.BasicAnatomy);
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.BasicFirstAid);
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.EmergencyMedicine);
        Find.ResearchManager.FinishProject(KnownResearchProjectDefOf.EpinephrineSynthesis);
    }
}