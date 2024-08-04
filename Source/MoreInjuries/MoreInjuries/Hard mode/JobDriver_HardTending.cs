using MoreInjuries.HealthConditions;
using MoreInjuries.HealthConditions.HeavyBleeding.Hemostat;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MoreInjuries;

// seems to be just first aid, contains some weird hemostat stuff
public class JobDriver_HardTending : JobDriver
{

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    public List<Thing> medicalDevices = [];

    protected override IEnumerable<Toil> MakeNewToils()
    {
        Pawn patient = (Pawn)TargetA.Thing;
        Pawn actor = GetActor();

        Toil t0 = new()
        {
            actor = actor
        };

        if (actor.Position.DistanceTo(patient.Position) > 1f)
        {
            Toil t1 = Toils_Goto.GotoCell(patient.Position, PathEndMode.Touch);

            yield return t1;
        }

        List<Thing> devices = actor.inventory.innerContainer.Where(x => x.def.HasModExtension<HemostatModExtension>()).ToList();

        if (!patient.ThingsInRange().Where(x => x.def.HasModExtension<HemostatModExtension>()).EnumerableNullOrEmpty())
        {
            devices.AddRange(patient.ThingsInRange().Where(x => x.def.HasModExtension<HemostatModExtension>()));
        }

        foreach (BetterInjury injury in patient.health.hediffSet.GetHediffsTendable().Where(x => x is BetterInjury && x.Part?.depth == BodyPartDepth.Outside).OrderBy(x => x.BleedRate))
        {
            if (/*injury.Bleeding &&*/ injury.BleedRate > 0.15f && injury.IsBase)
            {
                Thing fastestTendDevice = devices.MinBy(x => x.def.GetModExtension<HemostatModExtension>().ApplyTime);

                HemostatModExtension deviceModExt = fastestTendDevice.def.GetModExtension<HemostatModExtension>();

                Toil ToilApply = Toils_General.Wait((int)(deviceModExt.ApplyTime / actor.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation)));

                // TODO: are we sure that we want to decrease the stack count before it has been applied (would cause waste if aborted)?
                ToilApply.AddPreInitAction(fastestTendDevice.DecreaseStack);

                ToilApply.AddFinishAction(() =>
                {
                    injury.IsHemostatApplied = true;

                    injury.HemostatMultiplier = deviceModExt.CoagulationMultiplier;

                    IEnumerable<BetterInjury?> smallInjuries = patient.health.hediffSet.GetHediffsTendable().Where(x =>
                    x is BetterInjury
                    && x != injury
                    && x.BleedRate > 0
                    && x.BleedRate < 0.2f
                    && x.Part == injury.Part).Select(x => x as BetterInjury).Where(x => !x.IsHemostatApplied);

                    foreach (BetterInjury? injur in smallInjuries)
                    {
                        injur.IsBase = false;
                        injur.IsHemostatApplied = true;
                        injury.HemostatMultiplier = 0f;
                    }
                });

                yield return ToilApply;
            }
        }
    }
}
