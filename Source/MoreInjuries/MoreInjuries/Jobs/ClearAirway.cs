using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoreInjuries.Jobs;

internal class ClearAirway : JobDriver
{

    protected Pawn Patient
    {
        get
        {
            return (Pawn)job.GetTarget(TargetIndex.A).Thing;
        }
    }

    protected Pawn Doctor
    {
        get
        {
            return pawn;
        }
    }

    public Thing suctiondevice
    {
        get
        {
            return job.targetB.Thing;
        }
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return Doctor.Reserve(Patient, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnAggroMentalState(TargetIndex.A);

        Toil toil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return toil;
        Toil toil2 = Toils_General.Wait(320);
        toil2.AddFinishAction(delegate
        {
            Patient.health.RemoveHediff(Patient.health.hediffSet.hediffs.Find(AAA => AAA.def == Caula_DefOf.ChokingOnBlood));
        });
        yield return toil2;
        yield break;
    }

    private const TargetIndex targetInd = TargetIndex.A;
}
