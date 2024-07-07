using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoreInjuries.Hemostat;

public class UseHemostat : JobDriver
{
    public Pawn Patient => (Pawn)job.GetTarget(TargetIndex.A).Thing;

    public Pawn Doctor => pawn;

    public override bool TryMakePreToilReservations(bool errorOnFailed) => Doctor.Reserve(Patient, job, 1, 1, null, errorOnFailed);

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnAggroMentalState(TargetIndex.A);
        Toil toil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return toil;

        HemostatModExtension varA1 = TargetB.Thing.def.GetModExtension<HemostatModExtension>();

        Toil toil2 = Toils_General.Wait(varA1.ApplyTime);
        toil2.AddFinishAction(delegate
        {
            BetterInjury varC = Patient.TryGetComp<hemostat_comp>().injur;

            varC.isBase = false;

            varC.BleedRateSet = Patient.TryGetComp<hemostat_comp>().tagged_float;

            //Hediff hedf = HediffMaker.MakeHediff(HemoDefOf.hemostatised, Patient, varC.Part);

            //hedf.Severity = 1f;

            //hedf.TryGetComp<HemoHefComp>().injur = varC;

            //Patient.health.AddHediff(hedf);

            varC.hemod = true;

            if (TargetB.Thing.stackCount > 0)
            {
                --TargetB.Thing.stackCount;
            }
            else
            {
                TargetB.Thing.Destroy();
            }

            if (TargetB.Thing.stackCount == 0)
            {
                TargetB.Thing.Destroy();
            }
        });
        yield return toil2;
        yield break;
    }
}
