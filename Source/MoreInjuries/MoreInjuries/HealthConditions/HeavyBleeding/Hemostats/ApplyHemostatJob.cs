using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Hemostats;

// expects:
// - targetA: patient
// - targetB: hemostat
public class ApplyHemostatJob : JobDriver
{
    public Pawn Patient => (Pawn)TargetA.Thing;

    public Pawn Doctor => pawn;

    public override bool TryMakePreToilReservations(bool errorOnFailed) =>
        Doctor.Reserve(TargetA, job, maxPawns: 1, stackCount: 1, layer: null, errorOnFailed);

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnAggroMentalState(TargetIndex.A);
        Toil toilGotoPatient = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return toilGotoPatient;

        HemostatModExtension hemostatProperties = TargetB.Thing.def.GetModExtension<HemostatModExtension>();

        Toil toilApplyHempstat = Toils_General.Wait(hemostatProperties.ApplyTime);
        toilApplyHempstat.AddFinishAction(() =>
        {
            Pawn patient = Patient;
            HemostatThingComp? hemostatComp = patient.TryGetComp<HemostatThingComp>();
            BetterInjury injury = hemostatComp?.InjuryContext
                ?? throw new NullReferenceException("No HemostatComp found in the current context");

            injury.IsBase = false;
            injury.OverriddenBleedRate = injury.Severity * injury.def.injuryProps.bleedRate * injury.Part.def.bleedRate * hemostatProperties.CoagulationMultiplier;
            injury.IsHemostatApplied = true;

            Thing hemostat = TargetB.Thing;
            hemostat.DecreaseStack();
        });
        yield return toilApplyHempstat;
        yield break;
    }
}
