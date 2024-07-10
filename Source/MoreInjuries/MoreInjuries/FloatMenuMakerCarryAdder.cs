using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse.AI;
using HarmonyLib;
using Verse;
using UnityEngine;

namespace MoreInjuries;

[HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
public static class FloatMenuMakerCarryAdder
{
    // Token: 0x06000009 RID: 9 RVA: 0x00002150 File Offset: 0x00000350
    [HarmonyPostfix]
    public static void AddHumanlikeOrdersPostfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
    {
        bool flag = !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || pawn.WorkTagIsDisabled(WorkTags.Caring) || pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) || !pawn.workSettings.WorkIsActive(WorkTypeDefOf.Doctor);
        if (!flag)
        {
            foreach (LocalTargetInfo localTargetInfo in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true, null))
            {
                Pawn target = (Pawn)localTargetInfo.Thing;
                bool flag2 = !target.health.hediffSet.HasHediff(MoreInjuriesHediffDefOf.ChokingOnBlood);
                if (pawn.inventory.innerContainer.ToList().FindAll(AAA => AAA.def == MoreInjuriesHediffDefOf.suctiondevice) != null)
                {
                    bool flag69nice = pawn.inventory.innerContainer.ToList().FindAll(AAA => AAA.def == MoreInjuriesHediffDefOf.suctiondevice).Count > 0;
                    if (flag69nice)
                    {
                        if (!flag2)
                        {
                            bool flag3 = !pawn.CanReserveAndReach(target, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true);
                            if (!flag3)
                            {
                                JobDef stabilizeJD = MoreInjuriesHediffDefOf.ClearAirway;
                                Action action = delegate ()
                                {
                                    Job job = new(stabilizeJD, target);
                                    job.count = 1;
                                    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                                };
                                string label = "Clear airways";
                                opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, MenuOptionPriority.RescueOrCapture, null, target, 0f, null, null), pawn, target, "ReservedBy"));
                            }
                        }
                    }
                }
            }
        }
    }
}