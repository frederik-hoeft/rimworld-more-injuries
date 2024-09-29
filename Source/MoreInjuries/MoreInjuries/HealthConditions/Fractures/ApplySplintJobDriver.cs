using System.Collections.Generic;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Fractures;

public class ApplySplintJobDriver : JobDriver
{
    private Pawn Patient => (Pawn)job.targetA.Thing;

    private Pawn Doctor => pawn;

    public override bool TryMakePreToilReservations(bool errorOnFailed) => Doctor.Reserve(Patient, job, maxPawns: 1, stackCount: -1, layer: null, errorOnFailed);

    protected override IEnumerable<Toil> MakeNewToils()
    {
        Toil gotoPatientToil = Toils_Goto.Goto(TargetIndex.A, PathEndMode.ClosestTouch);
        gotoPatientToil.AddFinishAction(() => Patient.jobs.posture = PawnPosture.LayingOnGroundFaceUp);
        yield return gotoPatientToil;

        float medicalSkill = Doctor.skills.GetSkill(SkillDefOf.Medicine).Level;
        float manipulationCapacity = Doctor.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation);

        if (manipulationCapacity < 0.02f)
        {
            manipulationCapacity = 0.01f;
        }

        // TODO: shouldn't we multiply by manipulationCapacity here? isn't that a percentage between 0 and 1?
        Toil tendPatientToil = Toils_General.Wait(ticks: (int)Math.Round(480f / medicalSkill / manipulationCapacity));
        tendPatientToil.AddFinishAction(() =>
        {
            Hediff? fracture = Patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.Fracture);
            if (fracture?.Part is not null)
            {
                Hediff healingFracture = HediffMaker.MakeHediff(KnownHediffDefOf.FractureHealing, Patient, fracture.Part);
                healingFracture.Severity = 1f;
                Patient.health.AddHediff(healingFracture);
                // NOTE: previously, all hediffs with defname "Fracture" were removed here, now we only remove the one we're healing
                Patient.health.RemoveHediff(fracture);
            }
        });
        yield return tendPatientToil;
    }
}
