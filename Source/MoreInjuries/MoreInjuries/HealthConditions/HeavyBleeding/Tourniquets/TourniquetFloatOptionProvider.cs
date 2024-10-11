using MoreInjuries.KnownDefs;
using MoreInjuries.Things;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

internal class TourniquetFloatOptionProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler, ICompGetGizmosExtraHandler
{
    public bool IsEnabled => true;

    public void AddGizmosExtra(UIBuilder<Gizmo> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Target;
        if (patient.Downed && patient.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation) < 0.2f)
        {
            // pawn is downed and has too low manipulation to do anything
            return;
        }
        if (MedicalDeviceHelper.GetReasonForDisabledProcedure(selectedPawn, patient, JobDriver_UseTourniquet.JOB_LABEL) is not null)
        {
            // no tourniquet available or the selected pawn can't use it
            return;
        }
        string jobDescription = patient.Downed
            ? "While downed, the pawn still can attempt applying a tourniquet to themselves"
            : "Apply a tourniquet to themselves";
        builder.Options.Add(new Command_Action
        {
            defaultLabel = JobDriver_UseTourniquet.JOB_LABEL,
            defaultDesc = jobDescription,
            icon = ContentFinder<Texture2D>.Get("UI/tourniquet_gizmo", true),
            action = () =>
            {
                Thing? tourniquet = MedicalDeviceHelper.FindMedicalDevice(patient, patient, KnownThingDefOf.Tourniquet, fromInventoryOnly: true);

                // there will never be more than 5 limbs to apply a tourniquet to...
                List<FloatMenuOption> options = new(capacity: 5);

                bool pawnKnowsWhatTheyreDoing = PawnKnowsWhatTheyreDoing(selectedPawn);
                foreach (BodyPartRecord bodyPart in GetLimbs(patient))
                {
                    if (patient.health.hediffSet.hediffs.Any(hediff => hediff.Part == bodyPart && hediff.def == KnownHediffDefOf.TourniquetApplied))
                    {
                        options.Add(new FloatMenuOption($"{JobDriver_RemoveTourniquet.JOB_LABEL} from {bodyPart.Label.Colorize(Color.red)}", patient.Downed
                            ? () => JobDriver_RemoveTourniquet.ApplyDevice(patient, bodyPart)
                            : JobDriver_RemoveTourniquet.GetDispatcher(selectedPawn, patient, bodyPart).StartJob));
                    }
                    else if (tourniquet is not null && (bodyPart.def != KnownBodyPartDefOf.Neck || !pawnKnowsWhatTheyreDoing))
                    {
                        options.Add(new FloatMenuOption($"{JobDriver_UseTourniquet.JOB_LABEL} to {bodyPart.Label.Colorize(Color.green)}", patient.Downed
                            ? () => JobDriver_UseTourniquet.ApplyDevice(patient, tourniquet, bodyPart)
                            : JobDriver_UseTourniquet.GetDispatcher(selectedPawn, patient, tourniquet, bodyPart).StartJob));
                    }
                }
                if (options.Count == 0)
                {
                    options.Add(new FloatMenuOption("No tourniquets available", null));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }
        });
    }

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Target;
        if (!builder.Keys.Contains(UITreatmentOption.UseTourniquet))
        {
            builder.Keys.Add(UITreatmentOption.UseTourniquet);

            Thing? tourniquet = MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.Tourniquet);
            if (MedicalDeviceHelper.GetReasonForDisabledProcedure(selectedPawn, patient, JobDriver_UseTourniquet.JOB_LABEL) is string failure)
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
                return;
            }

            bool pawnKnowsWhatTheyreDoing = PawnKnowsWhatTheyreDoing(selectedPawn);
            int i = 0;
            foreach (BodyPartRecord bodyPart in GetLimbs(patient))
            {
                Logger.LogDebug($"Checking {bodyPart.Label} for tourniquet application ({i++})");
                if (patient.health.hediffSet.hediffs.Any(hediff => hediff.Part == bodyPart && hediff.def == KnownHediffDefOf.TourniquetApplied))
                {
                    builder.Options.Add(new FloatMenuOption($"{JobDriver_RemoveTourniquet.JOB_LABEL} from {bodyPart.Label.Colorize(Color.red)}", 
                        JobDriver_RemoveTourniquet.GetDispatcher(selectedPawn, patient, bodyPart).StartJob));
                }
                else if (tourniquet is not null && (bodyPart.def != KnownBodyPartDefOf.Neck || !pawnKnowsWhatTheyreDoing))
                {
                    builder.Options.Add(new FloatMenuOption($"{JobDriver_UseTourniquet.JOB_LABEL} to {bodyPart.Label.Colorize(Color.green)}", 
                        JobDriver_UseTourniquet.GetDispatcher(selectedPawn, patient, tourniquet, bodyPart).StartJob));
                }
            }
        }
    }

    private static IEnumerable<BodyPartRecord> GetLimbs(Pawn patient) => patient.health.hediffSet.GetNotMissingParts()
        .Where(bodyPart => bodyPart.def == BodyPartDefOf.Shoulder
            || bodyPart.def == BodyPartDefOf.Leg
            // a nice little easter egg for the less-gifted doctors out there :)
            || bodyPart.def == KnownBodyPartDefOf.Neck);

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