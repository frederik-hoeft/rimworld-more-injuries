using Verse;

namespace MoreInjuries.Versioning.Migrations;

public interface IVersionMigration : IExposable, ILoadReferenceable
{
    int Version { get; }

    void Migrate();

    void Execute(string signal);
}