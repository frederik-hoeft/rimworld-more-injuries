using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MoreInjuries.KnownDefs;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public class TourniquetThingComp : ThingComp
{
    public BodyPartRecord? TargetedBodyPart { get; private set; }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        Pawn patient = (Pawn)parent;
        if (patient.Downed && patient.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation) < 0.2f)
        {
            // pawn is downed and has too low manipulation to do anything
            yield break;
        }
        if (!TryFindTourniquet(patient, doctor: patient, out Thing? tourniquet))
        {
            // no tourniquet available
            yield break;
        }
        const string JOB_LABEL = "Apply tourniquet";
        string jobDescription = patient.Downed
            ? "While downed, the pawn still can attempt applying a tourniquet to themselves" 
            : "Apply a tourniquet to themselves";
        yield return new Command_Action
        {
            defaultLabel = JOB_LABEL,
            defaultDesc = jobDescription,
            icon = ContentFinder<Texture2D>.Get("UI/tour_gizmo", true),
            action = () =>
            {
                // there will never be more than 4 limbs to apply a tourniquet to...
                List<FloatMenuOption> options = new(capacity: 4);
                IEnumerable<BodyPartRecord> limbs = patient.health.hediffSet.GetNotMissingParts().Where(bodyPart => 
                    bodyPart.def == BodyPartDefOf.Shoulder 
                    || bodyPart.def == BodyPartDefOf.Leg);

                foreach (BodyPartRecord bodyPart in limbs)
                {
                    options.Add(new FloatMenuOption($"Apply a tourniquet to {bodyPart.Label.Colorize(Color.green)}", async () =>
                    {
                        TargetedBodyPart = bodyPart;
                        if (!patient.Downed)
                        {
                            Job applyTourniquet = new()
                            {
                                def = KnownJobDefOf.ApplyTourniquet,
                                targetA = patient,
                                targetB = tourniquet
                            };
                            patient.jobs.StartJob(applyTourniquet, JobCondition.InterruptForced);
                            return;
                        }
                        // can't really run a job since the pawn is downed, so we'll just run the job driver directly
                        // when running the "real" job driver, a 1 second delay is added to simulate the time it takes to apply a tourniquet
                        // it makes no sense for the tourniquet to be applied instantly, especially when the pawn is downed
                        // so we attempt to add an even longer delay here
                        // TODO: the theory is there, but no idea if Unity actually respects this (do some testing!)
                        if (SynchronizationContext.Current is not null)
                        {
                            Logger.LogVerbose("Trusting Unity's synchronization context implementation to post ApplyTourniquet job continuation back to the main thread after asynchronous delay");
                            // as this is an *async void* that isn't awaited, we can only do this if we are certain that there is a synchronization context
                            // so that the continuation can be marshaled back onto the main thread
                            // if there is no synchronization context, we can't do this, because async/await will cause concurrency issues and we can't just block the main thread either
                            await Task.Delay(2000).ConfigureAwait(continueOnCapturedContext: true);
                            _ = SynchronizationContext.Current ?? throw new InvalidOperationException("Failed to marshal async void continuation back to the main thread :C");
                            Logger.LogVerbose("Synchronization context is still present, indicating a successful task continuation on the main thread");
                        }
                        else
                        {
                            Logger.Log("No synchronization context present, cannot apply tourniquet delay without risking concurrency issues");
                        }
                        ApplyTourniquetJobDriver.ApplyTourniquet(patient, tourniquet!, this);
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }
        };
    }

    // these are the right-click options, I think (a pawn is selected, and we click on a patient pawn)
    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectedPawn)
    {
        Pawn patient = (Pawn)parent;

        if (!patient.Downed && patient.Faction.HostileTo(Faction.OfPlayer))
        {
            // hostile pawns probably won't allow us to apply a tourniquet to them when they're not downed
            yield break;
        }
        if (TryFindTourniquet(patient, doctor: selectedPawn, out Thing? tourniquet))
        {
            bool pawnKnowsWhatTheyreDoing = PawnKnowsWhatTheyreDoing(selectedPawn);
            IEnumerable<BodyPartRecord> limbs = patient.health.hediffSet.GetNotMissingParts().Where(bodyPart =>
                bodyPart.def == BodyPartDefOf.Shoulder
                || bodyPart.def == BodyPartDefOf.Leg
                // a nice little easter egg for the less-gifted doctors out there :)
                || !pawnKnowsWhatTheyreDoing && bodyPart.def == KnownBodyPartDefOf.Neck);

            foreach (BodyPartRecord bodyPart in limbs)
            {
                yield return new FloatMenuOption("Apply a tourniquet to " + bodyPart.Label.Colorize(Color.green), () =>
                {
                    TargetedBodyPart = bodyPart;

                    Job applyTourniquet = new() 
                    { 
                        def = KnownJobDefOf.ApplyTourniquet, 
                        targetA = patient, 
                        targetB = tourniquet
                    };
                    selectedPawn.jobs.StartJob(applyTourniquet, JobCondition.InterruptForced);
                });
            }
        }

        foreach (Hediff hediff in patient.health.hediffSet.hediffs.Where(hediff => hediff.def == KnownHediffDefOf.TourniquetApplied))
        {
            yield return new FloatMenuOption
            (
                label: $"Remove tourniquet from {hediff.Part.Label.Colorize(new Color(130, 0, 130))}",
                action: () => patient.health.RemoveHediff(hediff)
            );
        }
    }

    private static bool TryFindTourniquet(Pawn patient, Pawn doctor, out Thing? tourniquet)
    {
        // prefer the doctor's inventory first
        tourniquet = doctor.inventory.innerContainer.FirstOrDefault(thing => thing.def == KnownThingDefOf.Tourniquet);
        if (tourniquet is null && !ReferenceEquals(patient, doctor))
        {
            // otherwise, check the patient's inventory
            tourniquet = patient.inventory.innerContainer.FirstOrDefault(thing => thing.def == KnownThingDefOf.Tourniquet);
        }
        if (tourniquet is null)
        {
            // search in the patient's vicinity
            foreach (IntVec3 cell in patient.CellsAdjacent8WayAndInside())
            {
                tourniquet = cell.GetFirstThing(patient.Map, KnownThingDefOf.Tourniquet);
                if (tourniquet is not null)
                {
                    break;
                }
            }
        }
        return tourniquet is not null;
    }

    private static bool PawnKnowsWhatTheyreDoing(Pawn pawn)
    {
        int requiredSkillLevel = 3;
        if (pawn.story.traits.HasTrait(KnownTraitDefOf.SlowLearner))
        {
            requiredSkillLevel += 2;
        }
        Span<SkillRecordTracker> skillRecords =
        [
            new SkillRecordTracker(SkillDefOf.Medicine),
            new SkillRecordTracker(SkillDefOf.Intellectual)
        ];
        foreach (SkillRecord skill in pawn.skills.skills)
        {
            for (int i = 0; i < skillRecords.Length; i++)
            {
                if (skill.def == skillRecords[i].SkillDef && skill.Level < requiredSkillLevel)
                {
                    skillRecords[i].InsufficientSkill = true;
                    // there are only two entries we care about, so we can easily check the other one using some index math
                    if (skillRecords[Math.Abs(i - 1)].InsufficientSkill)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}

file struct SkillRecordTracker(SkillDef skillDef)
{
    public readonly SkillDef SkillDef = skillDef;
    public bool InsufficientSkill;
}