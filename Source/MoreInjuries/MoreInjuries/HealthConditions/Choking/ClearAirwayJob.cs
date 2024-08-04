using MoreInjuries.KnownDefs;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Choking;

internal class ClearAirwayJob : JobDriver
{
    private Pawn Patient => (Pawn)job.targetA.Thing;

    private Pawn Doctor => pawn;

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "provided, but currently unused")]
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
            if (Patient.health.hediffSet.hediffs.FirstOrDefault(hediff => hediff.def == KnownHediffDefOf.ChokingOnBlood) is Hediff chokingOnBlood)
            {
                Patient.health.RemoveHediff(chokingOnBlood);
            }
            // suction device is automatically destroyed after use (via XML)
        });
        yield return applySuctionDeviceToil;
        yield break;
    }
}
