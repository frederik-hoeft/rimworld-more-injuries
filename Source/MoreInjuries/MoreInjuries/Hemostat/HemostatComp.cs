using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.Hemostat;

public class HemostatComp : ThingComp
{
    public float BleedRate { get; set; }

    public BetterInjury Injury { get; set; }

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
    {
        if (MoreInjuriesMod.Settings.individualFloatMenus)
        {
            if (selPawn.inventory.innerContainer.Any(iran => iran.def.HasModExtension<HemostatModExtension>()))
            {

                Pawn parent = this.parent as Pawn;

                Thing hemo = selPawn.inventory.innerContainer.ToList().Find(iran => iran.def.HasModExtension<HemostatModExtension>());

                HemostatModExtension hemoprops = hemo.def.GetModExtension<HemostatModExtension>();

                foreach (BetterInjury injury in parent.health.hediffSet.GetHediffsTendable().OfType<BetterInjury>())
                {
                    if (injury.Part != null && injury.Bleeding && !injury.IsHemostatApplied && injury.Part.depth == BodyPartDepth.Outside)
                    {
                        float newbleedrate = injury.Severity * injury.def.injuryProps.bleedRate * injury.Part.def.bleedRate * hemoprops.CoagulationMultiplier;

                        BleedRate = newbleedrate;

                        yield return new FloatMenuOption("Use " + hemo.Label + " on: ".Colorize(new Color(26, 49, 20)) + injury.Label + " on " + injury.Part.Label.Colorize(new Color(26, 49, 20)),
                            delegate
                            {
                                Injury = injury;

                                Job jobber = new() { def = HemoDefOf.UseHemostat, targetA = parent, targetB = selPawn.inventory.innerContainer.ToList().Find(iran => iran.def.HasModExtension<HemostatModExtension>()) };

                                selPawn.jobs.StartJob(jobber, JobCondition.InterruptForced);
                            });
                    }
                }
            }
        }

        if (selPawn.inventory.innerContainer.Any(x => x.def.HasModExtension<HemostatModExtension>()) | !selPawn.ThingsInRange().Where(x => x.def.HasModExtension<HemostatModExtension>()).EnumerableNullOrEmpty()
            && !((Pawn)parent).health.hediffSet.GetHediffsTendable().EnumerableNullOrEmpty())
        {
            yield return new FloatMenuOption("Provide first aid",
                            delegate
                            {
                                selPawn.jobs.StartJob(new Job { def = FirstAidJobDefOf.FirstAid, targetA = parent }, JobCondition.InterruptForced);
                            });
        }
    }
}
