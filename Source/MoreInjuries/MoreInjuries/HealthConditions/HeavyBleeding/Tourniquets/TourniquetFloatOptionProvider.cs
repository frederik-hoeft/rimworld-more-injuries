using MoreInjuries.KnownDefs;
using MoreInjuries.Localization;
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
        string? failure = null;
        if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_UseTourniquet.JOB_LABEL_KEY) is MedicalDeviceHelper.DisabledProcedureCause cause)
        {
            failure = cause.FailureReason;
            if (!cause.IsSoftFailure)
            {
                // no tourniquet available or the selected pawn can't use it
                return;
            }
        }
        string jobDescription = patient.Downed
            ? "MI_TourniquetGizmo_Downed_Description".Translate()
            : "MI_TourniquetGizmo_Description".Translate();
        builder.Options.Add(new Command_Action
        {
            defaultLabel = JobDriver_UseTourniquet.JOB_LABEL_KEY.Translate(),
            defaultDesc = jobDescription,
            icon = ContentFinder<Texture2D>.Get("UI/tourniquet_gizmo", true),
            action = () =>
            {
                Thing? tourniquet = MedicalDeviceHelper.FindMedicalDevice(patient, patient, KnownThingDefOf.Tourniquet, fromInventoryOnly: true);

                // there will never be more than 5 limbs to apply a tourniquet to...
                List<FloatMenuOption> options = new(capacity: 5);
                if (failure is null)
                {
                    bool pawnKnowsWhatTheyreDoing = PawnKnowsWhatTheyreDoing(selectedPawn);
                    foreach (BodyPartRecord bodyPart in GetLimbs(patient))
                    {
                        if (patient.health.hediffSet.hediffs.Any(hediff => hediff.Part == bodyPart && hediff.def == KnownHediffDefOf.TourniquetApplied))
                        {
                            options.Add(new FloatMenuOption("MI_TourniquetGizmo_RemoveLabel".Translate(bodyPart.Label.Colorize(Color.red).Named(Named.Params.BODY_PART)), patient.Downed
                                ? () => JobDriver_RemoveTourniquet.ApplyDevice(patient, bodyPart)
                                : JobDriver_RemoveTourniquet.GetDispatcher(selectedPawn, patient, bodyPart).StartJob));
                        }
                        else if (tourniquet is not null && (bodyPart.def != KnownBodyPartDefOf.Neck || !pawnKnowsWhatTheyreDoing))
                        {
                            options.Add(new FloatMenuOption("MI_TourniquetGizmo_UseLabel".Translate(bodyPart.Label.Colorize(Color.green).Named(Named.Params.BODY_PART)), patient.Downed
                                ? () => JobDriver_UseTourniquet.ApplyDevice(patient, tourniquet, bodyPart)
                                : JobDriver_UseTourniquet.GetDispatcher(selectedPawn, patient, tourniquet, bodyPart).StartJob));
                        }
                    }
                    if (options.Count == 0)
                    {
                        options.Add(new FloatMenuOption("MI_UseTourniquetFailed_Unavailable".Translate(), null));
                    }
                }
                else
                {
                    options.Add(new FloatMenuOption(failure, null));
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
            if (tourniquet is not null && MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_UseTourniquet.JOB_LABEL_KEY) is { FailureReason: string failure })
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
                return;
            }

            bool pawnKnowsWhatTheyreDoing = PawnKnowsWhatTheyreDoing(selectedPawn);
            foreach (BodyPartRecord bodyPart in GetLimbs(patient))
            {
                if (patient.health.hediffSet.hediffs.Any(hediff => hediff.Part == bodyPart && hediff.def == KnownHediffDefOf.TourniquetApplied))
                {
                    builder.Options.Add(new FloatMenuOption(
                        "MI_TourniquetFloatMenu_RemoveLabel".Translate(
                            bodyPart.Label.Colorize(Color.red).Named(Named.Params.BODY_PART),
                            patient.Label.Colorize(Color.yellow).Named(Named.Params.PATIENT_NAME)),
                    JobDriver_RemoveTourniquet.GetDispatcher(selectedPawn, patient, bodyPart).StartJob));
                }
                else if (tourniquet is not null && selectedPawn.Drafted && (bodyPart.def != KnownBodyPartDefOf.Neck || !pawnKnowsWhatTheyreDoing))
                {
                    builder.Options.Add(new FloatMenuOption(
                        "MI_TourniquetFloatMenu_UseLabel".Translate(
                            bodyPart.Label.Colorize(Color.green).Named(Named.Params.BODY_PART),
                            patient.Label.Colorize(Color.yellow).Named(Named.Params.PATIENT_NAME)),
                        JobDriver_UseTourniquet.GetDispatcher(selectedPawn, patient, tourniquet, bodyPart).StartJob));
                }
            }
        }
    }

    private static IEnumerable<BodyPartRecord> GetLimbs(Pawn patient) => patient.health.hediffSet.GetNotMissingParts()
        .Where(bodyPart => (bodyPart.def == BodyPartDefOf.Shoulder
            || bodyPart.def == BodyPartDefOf.Leg
            // a nice little easter egg for the less-gifted doctors out there :)
            || bodyPart.def == KnownBodyPartDefOf.Neck)
            && !bodyPart.def.IsSolid(bodyPart, patient.health.hediffSet.hediffs));

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
                        return false;
                    }
                }
            }
        }
        return true;
    }
}

file struct SkillRecordTracker(SkillDef skillDef)
{
    public readonly SkillDef SkillDef = skillDef;
    public bool InsufficientSkill;
}