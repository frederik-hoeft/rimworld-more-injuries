using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.Hemostat;

public class hemostat_comp : ThingComp
{
    public float tagged_float;

    public BetterInjury injur;

    public List<BetterInjury> injuries_coagulateable;

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
    {
        if (MoreInjuriesMod.Settings.individualFloatMenus)
        {
            if (selPawn.inventory.innerContainer.Any(iran => iran.def.HasModExtension<HemostatModExtension>()))
            {

                Pawn parent = this.parent as Pawn;

                Thing hemo = selPawn.inventory.innerContainer.ToList().Find(iran => iran.def.HasModExtension<HemostatModExtension>());

                HemostatModExtension hemoprops = hemo.def.GetModExtension<HemostatModExtension>();

                foreach (BetterInjury injury in parent.health.hediffSet.GetInjuriesTendable())
                {
                    if (injury.Part != null && injury.Bleeding && !injury.hemod && injury.Part.depth == BodyPartDepth.Outside)
                    {
                        float newbleedrate = injury.Severity * injury.def.injuryProps.bleedRate * injury.Part.def.bleedRate * hemoprops.CoagulationMultiplier;

                        tagged_float = newbleedrate;

                        yield return new FloatMenuOption("Use " + hemo.Label + " on: ".Colorize(new Color(26, 49, 20)) + injury.Label + " on " + injury.Part.Label.Colorize(new Color(26, 49, 20)),
                            delegate
                            {
                                injur = injury;

                                Job jobber = new() { def = HemoDefOf.UseHemo, targetA = parent, targetB = selPawn.inventory.innerContainer.ToList().Find(iran => iran.def.HasModExtension<HemostatModExtension>()) };

                                selPawn.jobs.StartJob(jobber, JobCondition.InterruptForced);
                            }

                            );
                    }
                }
            }
        }

        if (selPawn.inventory.innerContainer.Any(x => x.def.HasModExtension<HemostatModExtension>()) | !selPawn.ThingsInRange().Where(x => x.def.HasModExtension<HemostatModExtension>()).EnumerableNullOrEmpty()
            && !((Pawn)parent).health.hediffSet.GetInjuriesTendable().EnumerableNullOrEmpty())
        {
            yield return new FloatMenuOption("Provide first aid",
                            delegate
                            {
                                selPawn.jobs.StartJob(new Job { def = FirstAidJobDefOf.FirstAid, targetA = parent }, JobCondition.InterruptForced);
                            });
        }
    }
}
