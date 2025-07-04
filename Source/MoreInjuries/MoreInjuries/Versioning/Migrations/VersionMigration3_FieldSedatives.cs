using MoreInjuries.KnownDefs;
using MoreInjuries.Localization;
using RimWorld;
using Verse;

namespace MoreInjuries.Versioning.Migrations;

internal sealed class VersionMigration3_FieldSedatives : VersionMigrationBase
{
    public override int Version => 3;

    public override void Migrate()
    {
        Logger.LogDebug("VersionMigration3_FieldSedatives: Migrate");
        Pawn? randomColonist = Find.CurrentMap?.mapPawns?.FreeColonists?.MaxBy(static pawn => pawn.skills?.GetSkill(SkillDefOf.Medicine)?.Level ?? -1);
        TaggedString title = "MI_Migration_Update_v3_Title".Translate();
        TaggedString text;
        if (randomColonist is not null)
        {
            text = "MI_Migration_Update_v3_Text_Personalized".Translate(randomColonist.Named(Named.Params.PAWN));
        }
        else
        {
            text = "MI_Migration_Update_v3_Text".Translate();
        }
        // show changelog
        ChoiceLetter_MigrateVersion letter = (ChoiceLetter_MigrateVersion)LetterMaker.MakeLetter(title, text, KnownLetterDefOf.VersionMigration);
        letter.VersionMigration = this;
        letter.Options =
        [
            new ChoiceLetter_MigrateVersion.Option(GENERIC_CONTINUE_SIGNAL),
        ];
        Find.LetterStack.ReceiveLetter(letter);
    }

    public override void Execute(string signal) => Logger.LogDebug($"{GetType().Name}: Execute: {signal}");
}
