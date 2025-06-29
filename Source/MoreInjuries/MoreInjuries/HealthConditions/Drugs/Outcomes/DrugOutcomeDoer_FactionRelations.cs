using MoreInjuries.BuildIntrinsics;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Outcomes;

[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class DrugOutcomeDoer_FactionRelations : DrugOutcomeDoer
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
