using MoreInjuries.Versioning.Migrations;
using Verse;

namespace MoreInjuries.Versioning;

[StaticConstructorOnStartup]
public static class MoreInjuries_VersionMigrations
{
    static MoreInjuries_VersionMigrations()
    {
        GameComponent_MoreInjuries.RegisterMigration(new VersionMigration1_ResearchProjects());
    }
}
