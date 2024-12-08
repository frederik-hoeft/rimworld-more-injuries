using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public class JobDriver_RemoveTourniquetFromDead : JobDriver_MedicalBase<Corpse>
{
    private const TargetIndex CORPSE_INDEX = TARGET_INDEX;

    private Corpse Corpse => (Corpse)job.targetA.Thing;

    protected override Corpse GetTarget(ref readonly LocalTargetInfo targetInfo) => (Corpse)targetInfo.Thing;

    protected override int BaseTendDuration => 120;

    protected override float BaseExperience => 50f;

    public override bool TryMakePreToilReservations(bool errorOnFailed) => 
        Doctor.Reserve(Corpse, job, errorOnFailed: errorOnFailed);

    private static bool HasTourniquet(Corpse corpse) =>
        corpse.InnerPawn.health.hediffSet.hediffs.Any(hediff => hediff.def == KnownHediffDefOf.TourniquetApplied);

    protected override void FinalizeTreatment(Pawn doctor, Corpse target, Thing? thing)
    {
        Pawn patient = target.InnerPawn;
        if (patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.TourniquetApplied, out Hediff? tourniquet))
        {
            patient.health.RemoveHediff(tourniquet);
            // spawn a tourniquet item on the ground
            Thing tourniquetThing = ThingMaker.MakeThing(KnownThingDefOf.Tourniquet);
            GenPlace.TryPlaceThing(tourniquetThing, target.Position, target.Map, ThingPlaceMode.Near);
        }
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        Pawn doctor = Doctor;
        Corpse corpse = Corpse;
        this.FailOnDespawnedNullOrForbidden(CORPSE_INDEX);
        AddEndCondition(() =>
        {
            if (HasTourniquet(corpse))
            {
                return JobCondition.Ongoing;
            }
            return JobCondition.Succeeded;
        });
        Toil gotoCorpseToil = Toils_Goto.GotoThing(CORPSE_INDEX, PathEndMode.Touch);
        yield return gotoCorpseToil;
        int ticks = CalculateTendDuration();
        Toil waitToil = Toils_General.Wait(ticks, face: CORPSE_INDEX);
        waitToil.WithProgressBarToilDelay(CORPSE_INDEX).PlaySustainerOrSound(SoundDef);
        waitToil.activeSkill = () => SkillDefOf.Medicine;
        waitToil.handlingFacing = true;
        waitToil.FailOn(() =>
        {
            if (!ReachabilityImmediate.CanReachImmediate(doctor, corpse.SpawnedParentOrMe, PathEndMode.Touch))
            {
                Logger.Warning($"Failed job {job.def} because {doctor} can't reach {corpse.InnerPawn.ToStringSafe()}");
                return true;
            }
            return false;
        });
        yield return waitToil;
        yield return FinalizeTreatmentToil();
        yield return Toils_Jump.JumpIf(gotoCorpseToil, () => HasTourniquet(corpse));
    }
}
