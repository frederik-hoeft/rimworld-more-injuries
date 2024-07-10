using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.Hemostat;

public class HemostatComp : ThingComp
{
    private static readonly Color _bodyPartLabelColor = new(26, 49, 20);
    private static readonly Color _injuryLabelColor = new(26, 49, 20);

    private static readonly ConcurrentDictionary<string, string> _bodyPartLabelCache = [];
    private static readonly ConcurrentDictionary<string, string> _injuryLabelCache = [];

    public BetterInjury? InjuryContext { get; set; }

    // these are the right-click options, I think (a pawn is selected, and we click on a patient pawn)
    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectedPawn)
    {
        Pawn patient = (Pawn)parent;
        Thing? hemostat = selectedPawn.inventory.innerContainer.FirstOrDefault(inventoryItem => inventoryItem.def.HasModExtension<HemostatModExtension>());

        // if the selected pawn has a hemostat in their inventory and the option to show "apply hemostat" is enabled
        if (MoreInjuriesMod.Settings.individualFloatMenus && hemostat is not null)
        {
            // ModExtension seems to store additional data for the defs (additional properties, inheritence through delegation)
            HemostatModExtension hemostatProperties = hemostat.def.GetModExtension<HemostatModExtension>();
            // grab all the tendable hediffs from the patient
            foreach (Hediff hediff in patient.health.hediffSet.GetHediffsTendable())
            {
                // do some pattern matching to check if the hediff is a BetterInjury and if it's an external injury that is bleeding and hasn't had a hemostat applied
                if (hediff is BetterInjury { Part.depth: BodyPartDepth.Outside, Bleeding: true, IsHemostatApplied: false } injury)
                {
                    // load colorized labels for the injury and the body part from cache, or create them if they don't exist
                    if (!_bodyPartLabelCache.TryGetValue(injury.Part.def.defName, out string? bodyPartLabel))
                    {
                        bodyPartLabel = injury.Part.Label.Colorize(_bodyPartLabelColor);
                        _bodyPartLabelCache.TryAdd(injury.Part.def.defName, bodyPartLabel);
                    }
                    if (!_injuryLabelCache.TryGetValue(injury.def.defName, out string? injuryLabel))
                    {
                        injuryLabel = injury.Label.Colorize(_injuryLabelColor);
                        _injuryLabelCache.TryAdd(injury.def.defName, injuryLabel);
                    }

                    StringBuilder labelBuilder = new(capacity: 128);
                    labelBuilder.Append("Apply ")
                        .Append(hemostat.Label)
                        .Append(" to: ")
                        .Append(injuryLabel)
                        .Append(" on ")
                        .Append(bodyPartLabel);

                    string label = labelBuilder.ToString();

                    yield return new FloatMenuOption(label, action: () =>
                    {
                        InjuryContext = injury;
                        Job job = new()
                        {
                            def = HemoDefOf.ApplyHemostatJob,
                            targetA = patient,
                            targetB = hemostat
                        };
                        selectedPawn.jobs.StartJob(job, JobCondition.InterruptForced);
                    });
                }
            }
        }

        // if the selected pawn has a hemostat in their inventory or one is in range of the patient
        if ((hemostat is not null || selectedPawn.ThingsInRange().Any(x => x.def.HasModExtension<HemostatModExtension>()))
            // and the patient has tendable hediffs
            && patient.health.hediffSet.HasTendableHediff())
        {
            yield return new FloatMenuOption("Provide first aid", action: () => selectedPawn.jobs.StartJob(new Job
            {
                def = FirstAidJobDefOf.FirstAid,
                targetA = parent
            }, JobCondition.InterruptForced));
        }
    }
}