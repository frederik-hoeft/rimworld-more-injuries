using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse.AI;
using HarmonyLib;
using Verse;
using UnityEngine;
using MoreInjuries.KnownDefs;

namespace MoreInjuries.HealthConditions.Choking;

[HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
public static class FloatMenuMakerMap_AddHumanlikeOrders_Patch
{
    [HarmonyPostfix]
    public static void AddHumanlikeOrdersPostfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
    {
        bool canRescue = pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation)
            && !pawn.WorkTagIsDisabled(WorkTags.Caring)
            && !pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor)
            && pawn.workSettings.WorkIsActive(WorkTypeDefOf.Doctor);
        if (!canRescue)
        {
            return;
        }
        bool hasSuctionDevice = pawn.inventory.innerContainer.Any(thing => thing.def == KnownThingDefOf.SuctionDevice);
        if (!hasSuctionDevice)
        {
            return;
        }
        foreach (LocalTargetInfo localTargetInfo in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), thingsOnly: true, source: null))
        {
            Pawn target = (Pawn)localTargetInfo.Thing;

            // add clear airway option if target is choking on blood and pawn can reach target
            if (target.health.hediffSet.HasHediff(KnownHediffDefOf.ChokingOnBlood)
                && pawn.CanReserveAndReach(target, PathEndMode.OnCell, Danger.Deadly, maxPawns: 1, stackCount: -1, layer: null, ignoreOtherReservations: true))
            {
                void startClearAirwayJob()
                {
                    Job job = new(KnownJobDefOf.ClearAirway, target)
                    {
                        count = 1
                    };
                    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                }
                opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption
                (
                    "Clear airways",
                    startClearAirwayJob,
                    MenuOptionPriority.RescueOrCapture,
                    mouseoverGuiAction: null,
                    revalidateClickTarget: target,
                    extraPartWidth: 0f,
                    extraPartOnGUI: null,
                    revalidateWorldClickTarget: null
                ), pawn, target, "ReservedBy"));
            }
        }
    }
}