using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using Verse.AI;
using MoreInjuries.Hemostat;

namespace MoreInjuries;

public static class DeviceToilsUtils
{
    public static List<Thing> ThingsInRange(this Thing vecsource)
    {
        List<Thing> result = new();
        foreach (IntVec3 vec3 in vecsource.CellsAdjacent8WayAndInside())
        {
            if (!vec3.GetThingList(vecsource.Map).NullOrEmpty())
            {
                result.AddRange(vec3.GetThingList(vecsource.Map));
            }
        }

        return result;
    }
    public static IEnumerable<Toil> UseBleedDecreaser(Pawn patient, Pawn actorset)
    {
        Toil t0 = new();

        t0.actor = actorset;

        Pawn actor = actorset;

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

                ToilApply.AddPreInitAction
                    (delegate
                    {
                        fastestTendDevice.DecreaseStack();
                    });

                ToilApply.AddFinishAction
                    (
                        delegate
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
                        }
                    );

                yield return ToilApply;
            }
        }
    }
}
