using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.Hemostat;

public class HemostatComp : ThingComp
{
    private static readonly Color _bodyPartLabelColor = new(26, 49, 20);
    private static readonly Color _injuryLabelColor = new(26, 49, 20);

    // frozen dictionary pulled in via NuGet (thanks for nothing, ancient .NET version :P)
    private static FrozenDictionary<string, string> _bodyPartLabelCache = new Dictionary<string, string>().ToFrozenDictionary();
    private static FrozenDictionary<string, string> _injuryLabelCache = new Dictionary<string, string>().ToFrozenDictionary();

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
                    // load colorized labels for the injury and the body part from cache, or recomplie the caches if the entry is missing
                    // snap a stack-local copy of the cache reference
                    FrozenDictionary<string, string> bodyPartLabelCache = Volatile.Read(ref _bodyPartLabelCache);
                    if (!bodyPartLabelCache.TryGetValue(injury.Part.def.defName, out string? bodyPartLabel))
                    {
                        bodyPartLabel = injury.Part.Label.Colorize(_bodyPartLabelColor);
                        // re-compile the cache with the new entry added (should only happen once per body part, after that everything is cached and read-only)
                        // volatile write back to the static field, may overwrite other threads' changes leading to eventual complete-ness
                        Volatile.Write(ref _bodyPartLabelCache, bodyPartLabelCache.CopyAndAdd(injury.Part.def.defName, bodyPartLabel));
                    }
                    FrozenDictionary<string, string> injuryLabelCache = Volatile.Read(ref _injuryLabelCache);
                    if (!injuryLabelCache.TryGetValue(injury.def.defName, out string? injuryLabel))
                    {
                        injuryLabel = injury.Label.Colorize(_injuryLabelColor);
                        Volatile.Write(ref _injuryLabelCache, injuryLabelCache.CopyAndAdd(injury.def.defName, injuryLabel));
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

file static class FrozenDictionaryExtensions
{
    public static FrozenDictionary<TKey, TValue> CopyAndAdd<TKey, TValue>(this FrozenDictionary<TKey, TValue> source, TKey newKey, TValue newValue) where TKey : notnull
    {
        KeyValuePair<TKey, TValue>[] newData =
        [
            .. source,
            new KeyValuePair<TKey, TValue>(newKey, newValue)
        ];
        return newData.ToFrozenDictionary();
    }
}