using System.Collections.Generic;
using System.Linq;
using MoreInjuries.Debug;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public class ApplyTourniquetJobDriver : JobDriver
{
    private Pawn Patient => (Pawn)job.targetA.Thing;

    private Pawn Doctor => pawn;

    public override bool TryMakePreToilReservations(bool errorOnFailed) => 
        Doctor.Reserve(Patient, job, maxPawns: 1, stackCount: 1, layer: null, errorOnFailed: errorOnFailed);

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnAggroMentalState(TargetIndex.A);

        Toil gotoPatientToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return gotoPatientToil;

        Patient.jobs.posture = PawnPosture.LayingOnGroundFaceUp;
        Toil applyTourniquetToil = Toils_General.Wait(60);
        applyTourniquetToil.AddFinishAction(() => ApplyTourniquet(Patient, TargetB.Thing));
        yield return applyTourniquetToil;
        yield break;
    }

    internal static void ApplyTourniquet(Pawn pawn, Thing tourniquet) => ApplyTourniquet(pawn, tourniquet, pawn.GetComp<TourniquetThingComp>());

    internal static void ApplyTourniquet(Pawn pawn, Thing tourniquet, TourniquetThingComp tourniquetComp)
    {
        DebugAssert.NotNull(tourniquetComp, nameof(tourniquetComp));
        DebugAssert.NotNull(tourniquetComp.TargetedBodyPart, nameof(tourniquetComp.TargetedBodyPart));

        BodyPartRecord bodyPart = tourniquetComp.TargetedBodyPart!;

        List<Hediff_Injury> treatedInjuries = [];
        foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
        {
            // tourniquets can only be applied to bleeding injuries that are tendable
            if (hediff is Hediff_Injury { Bleeding: true } injury 
                && injury.TendableNow() 
                // and the injury must be on the targeted body part or one of its children
                && (injury.Part == bodyPart || bodyPart.parts is { Count: > 0 }
                    && (bodyPart.parts.Contains(injury.Part)
                        || bodyPart.parts.Any(part => part.parts?.Contains(injury.Part) is true))))
            {
                treatedInjuries.Add(injury);
                if (injury is BetterInjury betterInjury)
                {
                    betterInjury.IsBase = false;
                    betterInjury.OverriddenBleedRate = 0.01f;
                }
            }
        }
        Hediff appliedTourniquetHediff = HediffMaker.MakeHediff(KnownHediffDefOf.TourniquetApplied, pawn, bodyPart);
        appliedTourniquetHediff.Severity = 1.1f;
        if (appliedTourniquetHediff.TryGetComp(out TourniquetHediffComp comp))
        {
            comp.Injuries = treatedInjuries;
        }
        else
        {
            Log.Error("Failed to get TourniquetHediffComp from applied tourniquet hediff. Please report this bug to the maintainers of the More Injuries mod.");
        }
        pawn.health.AddHediff(appliedTourniquetHediff, bodyPart);
        if (tourniquet.stackCount > 1)
        {
            --tourniquet.stackCount;
        }
        else
        {
            tourniquet.Destroy();
        }
    }
}

