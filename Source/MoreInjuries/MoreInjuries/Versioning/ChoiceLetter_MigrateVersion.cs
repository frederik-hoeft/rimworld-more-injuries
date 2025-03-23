using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.Versioning;

public class ChoiceLetter_MigrateVersion : ChoiceLetter
{
    public override bool CanDismissWithRightClick => false;

    public override bool CanShowInLetterStack => !IsResolved;

    public bool IsResolved { get; private set; }

    public List<Option>? Options { get; set; }

    public override IEnumerable<DiaOption> Choices
    {
        get
        {
            if (Options is null)
            {
                yield break;
            }
            if (ArchivedOnly)
            {
                yield return Option_Close;
                yield break;
            }
            foreach (Option option in Options)
            {
                option.Parent = this;
                yield return new DiaOption(option.Label)
                {
                    action = option.Action,
                    resolveTree = true
                };
            }
        }
    }

    public record Option(string Label, Action Action)
    {
        public ChoiceLetter_MigrateVersion? Parent { get; set; }

        public void CompleteMigration()
        {
            Action.Invoke();
            if (Parent is not null)
            {
                Logger.LogDebug("ChoiceLetter_MigrateVersion: CompleteMigration");
                Parent.IsResolved = true;
                Find.LetterStack.RemoveLetter(Parent);
            }
        }
    }
}
