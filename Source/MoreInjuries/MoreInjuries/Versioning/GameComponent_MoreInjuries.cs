using MoreInjuries.Versioning.Migrations;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.Versioning;

#pragma warning disable CS9113 // Parameter is unread.
// required for the constructor to be called by the game
public class GameComponent_MoreInjuries(Game game) : GameComponent
#pragma warning restore CS9113 // Parameter is unread.
{
    private static readonly List<IVersionMigration> s_migrations = [];
    internal int version;

    public override void LoadedGame()
    {
        Logger.Log($"[GameComponent_MoreInjuries] MigrationManager loaded save file: current version is {version}. Checking for migrations to run ...");
        // run migrations
        foreach (IVersionMigration migration in s_migrations)
        {
            if (migration.Version > version)
            {
                Logger.Log($"[GameComponent_MoreInjuries] MigrationManager: Migrating from version {version} to {migration.Version}");
                migration.Migrate();
                version = migration.Version;
            }
        }
        Logger.Log("[GameComponent_MoreInjuries] MigrationManager: Migrations complete");
    }

    public static void RegisterMigration(IVersionMigration migration)
    {
        int i;
        for (i = 0; i < s_migrations.Count && s_migrations[i].Version < migration.Version; ++i) { }
        if (i < s_migrations.Count)
        {
            s_migrations.Insert(i, migration);
        }
        else
        {
            s_migrations.Add(migration);
        }
    }

    public override void ExposeData()
    {
        if (Scribe.mode is LoadSaveMode.Saving)
        {
            version = s_migrations.Max(static m => m.Version);
            Logger.Log($"[GameComponent_MoreInjuries] MigrationManager: Saving game with version {version}");
        }
        Scribe_Values.Look(ref version, "version", defaultValue: 0);
    }
}
