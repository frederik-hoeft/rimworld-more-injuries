using Verse;

namespace MoreInjuries.Versioning.Migrations;

internal abstract class VersionMigrationBase : IVersionMigration
{
    protected const string GENERIC_CONTINUE_SIGNAL = "MI_Migration_Signal_Generic_Continue";

    protected string? _loadID;

    public abstract int Version { get; }

    public abstract void Execute(string signal);

    public virtual void ExposeData()
    {
        Scribe_Values.Look(ref _loadID, "loadID", defaultValue: null);
    }

    public virtual string GetUniqueLoadID() => _loadID ??= $"MoreInjuries.{GetType().Name}_{Guid.NewGuid()}";

    public abstract void Migrate();
}
