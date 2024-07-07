using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.AI;
using UnityEngine;

namespace Oof
{
    [DefOf]
    public class FirstAidJobDefOf : DefOf
    {
        public static JobDef FirstAid;
    }
    public class JobDriver_HardTending : JobDriver
    {
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

		public List<Thing> medicalDevices = new List<Thing>();

        protected override IEnumerable<Toil> MakeNewToils()
        {
            return DeviceToilsUtils.UseBleedDecreaser((Pawn)this.TargetA.Thing, GetActor());
		}
    }

    public static class DeviceToilsUtils
    {
        public static List<Thing> ThingsInRange(this Thing vecsource)
        {
            List<Thing> result = new List<Thing>();
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
            var t0 = new Toil();

            t0.actor = actorset;

            var actor = actorset;

            if (actor.Position.DistanceTo(patient.Position) > 1f)
            {
                var t1 = Toils_Goto.GotoCell(patient.Position, PathEndMode.Touch);

                yield return t1;
            }

            List<Thing> devices = actor.inventory.innerContainer.Where(x => x.def.HasModExtension<HemostatModExtension>()).ToList();

            if (!patient.ThingsInRange().Where(x => x.def.HasModExtension<HemostatModExtension>()).EnumerableNullOrEmpty())
            {
                devices.AddRange(patient.ThingsInRange().Where(x => x.def.HasModExtension<HemostatModExtension>()));
            }

            foreach (BetterInjury injury in patient.health.hediffSet.GetHediffsTendable().Where(x => x is BetterInjury && x.Part?.depth == BodyPartDepth.Outside).OrderBy(x => x.BleedRate))
            {
                if (/*injury.Bleeding &&*/ injury.BleedRate > 0.15f && injury.isBase)
                {
                    var fastestTendDevice = devices.MinBy(x => x.def.GetModExtension<HemostatModExtension>().ApplyTime);

                    var deviceModExt = fastestTendDevice.def.GetModExtension<HemostatModExtension>();

                    var ToilApply = Toils_General.Wait((int)(deviceModExt.ApplyTime / actor.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation)));

                    ToilApply.AddPreInitAction
                        (delegate
                        {
                            fastestTendDevice.DecreaseStack();
                        });


                    ToilApply.AddFinishAction
                        (
                            delegate
                            {
                                injury.hemod = true;

                                //Log.Message(deviceModExt.CoagulationMultiplier + fastestTendDevice.Label);
                                injury.hemoMult = deviceModExt.CoagulationMultiplier;

                                var smallInjuries = patient.health.hediffSet.GetHediffsTendable().Where(x =>
                                x is BetterInjury
                                && x != injury
                                && x.BleedRate > 0
                                && x.BleedRate < 0.2f
                                && x.Part == injury.Part).Select(x => x as BetterInjury).Where(x => !x.hemod);

                                foreach (var injur in smallInjuries)
                                {
                                    injur.isBase = false;
                                    injur.hemod = true;
                                    injury.hemoMult = 0f;
                                }
                            }
                        );

                    yield return ToilApply;
                }
            }
        }
    }
}
