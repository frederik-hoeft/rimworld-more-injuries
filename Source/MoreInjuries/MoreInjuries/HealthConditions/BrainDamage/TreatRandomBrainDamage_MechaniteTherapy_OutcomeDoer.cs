using MoreInjuries.HealthConditions.MechaniteTherapy;
using MoreInjuries.Localization;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.BrainDamage;

public sealed class TreatRandomBrainDamage_MechaniteTherapy_OutcomeDoer : HediffComp_MechaniteTherapy_OutcomeDoer
{
    public override void DoOutcome(HediffComp_MechaniteTherapy parentComp)
    {
        Pawn pawn = parentComp.Pawn;
        if (!pawn.health.hediffSet.hediffs.TryRandomElement(static hediff => hediff?.def.HasModExtension<BrainDamageTreatmentProps_ModExtension>() is true, out Hediff? hediff))
        {
            Logger.Warning($"{nameof(TreatRandomBrainDamage_MechaniteTherapy_OutcomeDoer)} could not find a valid brain damage hediff on {pawn.LabelShort}");
            return;
        }
        BrainDamageTreatmentProps_ModExtension modExtension = hediff!.def.GetModExtension<BrainDamageTreatmentProps_ModExtension>();
        float severityAdjustment = modExtension.SeverityReductionRange.RandomInRange;
        float newSeverity = hediff.Severity - severityAdjustment;
        if (newSeverity < Mathf.Epsilon)
        {
            pawn.health.RemoveHediff(hediff);
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("MI_Message_BrainDamageTreatment_HediffRemoved"
                    .Translate(pawn.Named(Named.Params.PAWN), hediff.Named(Named.Params.HEDIFF)),
                    pawn, MessageTypeDefOf.PositiveEvent);
            }
        }
        else
        {
            hediff.Severity = newSeverity;
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("MI_Message_BrainDamageTreatment_HediffSeverityDecreased"
                    .Translate(pawn.Named(Named.Params.PAWN), hediff.Named(Named.Params.HEDIFF), Math.Round(severityAdjustment * 100f, 2).Named(Named.Params.PERCENT)),
                    pawn, MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
