using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.HeavyBleeding.Hemostats;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding;

public class StopBleedingJob : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

    protected override IEnumerable<Toil> MakeNewToils()
    {
        Pawn patient = (Pawn)TargetA.Thing;
        Pawn doctor = GetActor();

        if (doctor.Position.DistanceTo(patient.Position) > 1f)
        {
            Toil gotoPatientToil = Toils_Goto.GotoCell(patient.Position, PathEndMode.Touch);
            yield return gotoPatientToil;
        }

        List<Hemostat> hemostats =
        [
            .. doctor.inventory.innerContainer
                .Where(x => x.def.HasModExtension<HemostatModExtension>())
                .Select(thing => new Hemostat(thing, thing.def.GetModExtension<HemostatModExtension>())),
            .. patient.ThingsInRange()
                .Where(x => x.def.HasModExtension<HemostatModExtension>())
                .Select(thing => new Hemostat(thing, thing.def.GetModExtension<HemostatModExtension>())),
        ];
        IOrderedEnumerable<BetterInjury> injuries = patient.health.hediffSet.GetHediffsTendable()
            .Where(hediff => hediff is BetterInjury { Part.depth: BodyPartDepth.Outside, BleedRate: > 0.15f, IsBase: true })
            .Select(injury => (BetterInjury)injury)
            .OrderBy(x => x.BleedRate);

        foreach (BetterInjury injury in injuries)
        {
            Hemostat fastestTendDevice = hemostats.MinBy(hemostat => hemostat.ModExtension.ApplyTime);

            Toil applyHemostatToil = Toils_General.Wait((int)(fastestTendDevice.ModExtension.ApplyTime / doctor.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation)));

            applyHemostatToil.AddFinishAction(() =>
            {
                injury.IsHemostatApplied = true;
                injury.HemostatMultiplier = fastestTendDevice.ModExtension.CoagulationMultiplier;

                if (injury.Part is not null)
                {
                    IEnumerable<BetterInjury> relatedInjuries = patient.health.hediffSet.hediffs
                        .Where(hediff => hediff is BetterInjury { BleedRate: > 0f and < 0.2f, IsHemostatApplied: false } relatedInjury
                            && relatedInjury != injury
                            && relatedInjury.IsOnBodyPartOrChildren(injury.Part))
                        .Select(hediff => (BetterInjury)hediff);

                    foreach (BetterInjury relatedInjury in relatedInjuries)
                    {
                        relatedInjury.IsBase = false;
                        relatedInjury.IsHemostatApplied = true;
                        relatedInjury.HemostatMultiplier = 0f;
                    }
                }
                if (fastestTendDevice.Thing.DecreaseStack())
                {
                    hemostats.Remove(fastestTendDevice);
                }
            });

            yield return applyHemostatToil;
        }
    }
}

file readonly record struct Hemostat(Thing Thing, HemostatModExtension ModExtension);