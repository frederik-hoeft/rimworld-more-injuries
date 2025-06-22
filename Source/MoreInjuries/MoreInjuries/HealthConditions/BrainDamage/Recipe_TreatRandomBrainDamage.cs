using MoreInjuries.HealthConditions.MechaniteTherapy;
using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.BrainDamage;

public class Recipe_TreatRandomBrainDamage : Recipe_Surgery
{
    public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe) => [pawn.health.hediffSet.GetBrain()];

    public override bool AvailableOnNow(Thing thing, BodyPartRecord? part = null)
    {
        if (thing is not Pawn pawn)
        {
            return false;
        }

        TreatmentInfo treatmentInfo = GetTreatmentOptions(pawn);
        bool available = treatmentInfo.TreatmentPossible && base.AvailableOnNow(thing, part);
        return available;
    }

    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        if (billDoer is null)
        {
            Logger.Warning($"{nameof(Recipe_TreatRandomBrainDamage)} was called with a null {nameof(billDoer)}");
            return;
        }
        if (GetTreatmentOptions(pawn) is not { TreatmentPossible: true, ModExtension: var modExtension })
        {
            Logger.Warning($"{nameof(Recipe_TreatRandomBrainDamage)} could not start treatment on {pawn.LabelShort} because no brain damage hediffs are present or treatment is already in progress.");
            return;
        }
        TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
        Hediff mechaniteTherapy = HediffMaker.MakeHediff(KnownHediffDefOf.MechaniteTherapy, pawn);
        if (!mechaniteTherapy.TryGetComp(out HediffComp_MechaniteTherapy? comp))
        {
            Logger.Error($"{nameof(Recipe_TreatRandomBrainDamage)}: Could not find HediffComp_MechaniteTherapy on {mechaniteTherapy.def.defName} for {pawn.LabelShort}");
            return;
        }
        comp!.OutcomeDoer = new TreatRandomBrainDamage_MechaniteTherapy_OutcomeDoer();
        // -1 severity per day, so Severity <=> DaysToComplete
        mechaniteTherapy.Severity = modExtension.DaysToComplete;
        pawn.health.AddHediff(mechaniteTherapy);
    }

    private TreatmentInfo GetTreatmentOptions(Pawn pawn)
    {
        bool treatmentInProgress = false;
        BrainDamageTreatmentProps_ModExtension? modExtension = null;
        for (int i = 0; i < pawn.health.hediffSet.hediffs.Count && (modExtension is null || !treatmentInProgress); ++i)
        {
            Hediff hediff = pawn.health.hediffSet.hediffs[i];
            if (hediff.def == KnownHediffDefOf.MechaniteTherapy)
            {
                treatmentInProgress = true;
                continue;
            }
            if (hediff.def.GetModExtension<BrainDamageTreatmentProps_ModExtension>() is { } extension)
            {
                modExtension = extension;
            }
        }

        return new TreatmentInfo(!treatmentInProgress && modExtension is not null, modExtension);
    }

    private readonly record struct TreatmentInfo
    (
        [property: MemberNotNullWhen(true, nameof(TreatmentInfo.ModExtension))] bool TreatmentPossible,
        BrainDamageTreatmentProps_ModExtension? ModExtension
    );
}
