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
        Toil toilGotoPatient = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return toilGotoPatient;

        HemostatModExtension varA1 = TargetB.Thing.def.GetModExtension<HemostatModExtension>();

        Toil toilApplyHempstat = Toils_General.Wait(varA1.ApplyTime);
        toilApplyHempstat.AddFinishAction(() =>
        {
            BetterInjury injury = Patient.TryGetComp<HemostatComp>().Injury;

            injury.IsBase = false;

            injury.OverriddenBleedRate = Patient.TryGetComp<HemostatComp>().BleedRate;

            //Hediff hedf = HediffMaker.MakeHediff(HemoDefOf.HemostatApplied, Patient, varC.Part);

            //hedf.Severity = 1f;

            //hedf.TryGetComp<HemoHefComp>().injur = varC;

            //Patient.health.AddHediff(hedf);

            injury.IsHemostatApplied = true;

            if (TargetB.Thing.stackCount > 0)
            {
                TargetB.Thing.stackCount--;
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
        yield return toilApplyHempstat;
        yield break;
    }
}
