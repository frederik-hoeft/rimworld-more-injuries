using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using RimWorld;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlBindable]
public partial class JobOutcomeDoer_FactionRelations : JobOutcomeDoer
{
    [XmlBinding<int>("goodwillChange", defaultValue: 0)]
    public partial int GoodwillChange { get; }

    [XmlBinding("historyEventDef")]
    public partial HistoryEventDef HistoryEventDef { get; }

    [XmlBinding<bool>("isViolation", defaultValue: false)]
    public partial bool IsViolation { get; }

    [XmlBinding<bool>("onlyIfFriendly", defaultValue: false)]
    public partial bool OnlyIfFriendly { get; }

    protected override bool DoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        if (patient.Faction is Faction factionToInform
            && (!factionToInform.IsPlayerSafe() || patient.IsQuestLodger())
            && !(OnlyIfFriendly && factionToInform.HostileTo(Faction.OfPlayer)))
        {
            Faction.OfPlayer.TryAffectGoodwillWith(factionToInform, GoodwillChange, canSendHostilityLetter: !factionToInform.temporary, reason: HistoryEventDef);
            if (IsViolation)
            {
                QuestUtility.SendQuestTargetSignals(patient.questTags, QuestUtility.QuestTargetSignalPart_SurgeryViolation, patient.Named("SUBJECT"));
            }
        }
        return true;
    }

    public override string ToString() =>
        $"{nameof(JobOutcomeDoer_FactionRelations)}(GoodwillChange: {GoodwillChange}, HistoryEventDef: {HistoryEventDef.defName}, IsViolation: {IsViolation})";
}
