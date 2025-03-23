namespace MoreInjuries.Versioning.Migrations;

public interface IVersionMigration
{
    int Version { get; }

    void Migrate();
}