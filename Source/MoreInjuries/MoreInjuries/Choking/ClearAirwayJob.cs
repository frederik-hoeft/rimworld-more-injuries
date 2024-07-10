using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoreInjuries.Choking;

internal class ClearAirwayJob : JobDriver
{
    private Pawn Patient => (Pawn)job.targetA.Thing;

    private Pawn Doctor => pawn;

    private Thing SuctionDevice => job.targetB.Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed) => Doctor.Reserve(Patient, job, maxPawns: 1, stackCount: -1, layer: null, errorOnFailed);

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnAggroMentalState(TargetIndex.A);

        Toil gotoPatientToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return gotoPatientToil;
        Toil applySuctionDeviceToil = Toils_General.Wait(320);
        applySuctionDeviceToil.AddFinishAction(() =>
        {
            if (Patient.health.hediffSet.hediffs.FirstOrDefault(hediff => hediff.def == MoreInjuriesHediffDefOf.ChokingOnBlood) is Hediff chokingOnBlood)
            {
                Patient.health.RemoveHediff(chokingOnBlood);
            }
            // suction device is automatically destroyed after use (via XML)
        });
        yield return applySuctionDeviceToil;
        yield break;
    }
}
