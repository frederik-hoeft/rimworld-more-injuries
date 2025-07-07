using RimWorld;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class JobOutcomeDoer_FactionRelations : JobOutcomeDoer
{
    // don't rename this field. XML defs depend on this name
    private readonly int goodwillChange = 0;
    // don't rename this field. XML defs depend on this name
    private readonly HistoryEventDef historyEventDef = default!;
    // don't rename this field. XML defs depend on this name
    private readonly bool isViolation = false;

    public int GoodwillChange => goodwillChange;

    public HistoryEventDef HistoryEventDef => historyEventDef;

    public bool IsViolation => isViolation;

    protected override bool DoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        if (patient.Faction is Faction factionToInform && (factionToInform != Faction.OfPlayer || patient.IsQuestLodger()))
        {
            Faction.OfPlayer.TryAffectGoodwillWith(factionToInform, goodwillChange, canSendHostilityLetter: !factionToInform.temporary, reason: historyEventDef);
            if (isViolation)
            {
                QuestUtility.SendQuestTargetSignals(patient.questTags, QuestUtility.QuestTargetSignalPart_SurgeryViolation, patient.Named("SUBJECT"));
            }
        }
        return true;
    }
}
