using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Choking;

public class PerformCprJob : JobDriver
{
    private Pawn Patient => (Pawn)job.targetA.Thing;

    private Pawn Doctor => pawn;

    public override bool TryMakePreToilReservations(bool errorOnFailed) => Doctor.Reserve(Patient, job, maxPawns: 1, stackCount: -1, layer: null, errorOnFailed);

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnAggroMentalState(TargetIndex.A);

        Toil gotoPatientToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        gotoPatientToil.AddFinishAction(() => Patient.jobs.posture = PawnPosture.LayingOnGroundFaceUp);
        yield return gotoPatientToil;

        Toil performCprToil = Toils_General.Wait(ticks: 60);
        int doctorMedicineSkill = Doctor.skills.skills.Find(o => o.def == SkillDefOf.Medicine).Level;
        performCprToil.AddFinishAction(() =>
        {
            Hediff? hediff = Patient.health.hediffSet.hediffs.FirstOrDefault(CanBeTreatedWithCpr);
            if (hediff is not null)
            {
                hediff.Severity -= doctorMedicineSkill * 1.35f / 100f;
            }
        });
        yield return performCprToil;

        AddEndCondition(() =>
        {
            if (Patient.health.hediffSet.hediffs.Any(CanBeTreatedWithCpr))
            {
                return JobCondition.Ongoing;
            }
            return JobCondition.Succeeded;
        });
    }

    private static bool CanBeTreatedWithCpr(Hediff hediff) =>
        hediff.def == MoreInjuriesHediffDefOf.ChokingOnBlood
        || hediff.def.defName is "HeartAttack";
}
