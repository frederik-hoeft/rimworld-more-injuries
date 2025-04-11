using MoreInjuries.Extensions;
using MoreInjuries.Versioning.Migrations;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.Versioning;

public class ChoiceLetter_MigrateVersion : ChoiceLetter
{
    private bool _isResolved;
    private IVersionMigration? _versionMigration;
    private List<Option>? _options;

    public override bool CanDismissWithRightClick => false;

    public override bool CanShowInLetterStack => !IsResolved;

    public bool IsResolved 
    { 
        get => _isResolved; 
        private set => _isResolved = value; 
    }

    public List<Option>? Options 
    { 
        get => _options;
        set => _options = value;
    }

    public IVersionMigration VersionMigration
    {
        get => Exposable.RequiredMember(_versionMigration, nameof(ChoiceLetter_MigrateVersion));
        set => _versionMigration = value;
    }

    public override IEnumerable<DiaOption> Choices
    {
        get
        {
            if (ArchivedOnly || Options is null)
            {
                yield return Option_Close;
                yield break;
            }
            foreach (Option option in Options)
            {
                option.Parent = this;
                yield return new DiaOption(option.Signal.Translate())
                {
                    action = option.CompleteMigration,
                    resolveTree = true
                };
            }
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref _isResolved, "isResolved", false);
        Scribe_Deep.Look(ref _versionMigration, "versionMigration", null);
        Scribe_Collections.Look(ref _options, "options", LookMode.Deep);
    }

    public sealed class Option(string signal) : IExposable
    {
        private string? _signal = signal;

        private Option() : this(null!) { }

        public string Signal
        {
            get => Exposable.RequiredMember(_signal, nameof(Option));
            set => _signal = value;
        }

        public ChoiceLetter_MigrateVersion? Parent { get; set; }

        public void CompleteMigration()
        {
            if (Parent is not null)
            {
                Parent.VersionMigration.Execute(Signal);
                Logger.LogDebug("ChoiceLetter_MigrateVersion: CompleteMigration");
                Parent.IsResolved = true;
                Find.LetterStack.RemoveLetter(Parent);
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref _signal, "signal", null);
        }
    }
}
