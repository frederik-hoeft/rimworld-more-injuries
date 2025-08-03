using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Localization;
using MoreInjuries.Things;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

internal sealed class TourniquetFloatOptionProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler, ICompGetGizmosExtraHandler
{
    public bool IsEnabled => true;

    public void AddGizmosExtra(UIBuilder<Gizmo> builder, Pawn selectedPawn)
    {
        if (!MoreInjuriesMod.Settings.EnableTourniquetGizmo || !KnownResearchProjectDefOf.BasicFirstAid.IsFinished)
        {
            return;
        }
        Pawn patient = parent.Target;
        if (patient.Downed && patient.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation) < 0.2f)
        {
            // pawn is downed and has too low manipulation to do anything
            return;
        }
        string? failure = null;
        // Ignore self-tend setting here to allow the selected pawn to use a tourniquet on themselves regardless of their self-tend medical setting.
        if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_UseTourniquet.JOB_LABEL_KEY, ignoreSelfTendSetting: true) is MedicalDeviceHelper.DisabledProcedureCause cause)
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
                    bool pawnKnowsWhatTheyreDoing = JobDriver_TourniquetBase.PawnKnowsWhatTheyreDoing(selectedPawn);

                    using BleedRateByLimbEnumerable bleedRateCache = BleedRateByLimbEnumerable.EvaluateLimbs(patient);
                    foreach ((BodyPartRecord bodyPart, float aggregatedBleedRate) in bleedRateCache)
                    {
                        if (patient.health.hediffSet.hediffs.Any(hediff => hediff.Part == bodyPart && hediff.def == KnownHediffDefOf.TourniquetApplied))
                        {
                            options.Add(new FloatMenuOption("MI_TourniquetGizmo_RemoveSafelyLabel".Translate(bodyPart.Label.Colorize(Color.red).Named(Named.Params.BODYPART)).Colorize(Color.white), patient.Downed
                                ? () => JobDriver_RemoveTourniquetSafely.ApplyDevice(patient, bodyPart)
                                : JobDriver_RemoveTourniquetSafely.GetDispatcher(selectedPawn, patient, bodyPart).StartJob));
                            options.Add(new FloatMenuOption("MI_TourniquetGizmo_RemoveQuicklyLabel".Translate(bodyPart.Label.Colorize(Color.red).Named(Named.Params.BODYPART)).Colorize(Color.white), patient.Downed
                                ? () => JobDriver_RemoveTourniquetQuickly.ApplyDevice(patient, bodyPart)
                                : JobDriver_RemoveTourniquetQuickly.GetDispatcher(selectedPawn, patient, bodyPart).StartJob));
                        }
                        else if (tourniquet is not null && (bodyPart.def != KnownBodyPartDefOf.Neck || !pawnKnowsWhatTheyreDoing))
                        {
                            options.Add(new FloatMenuOption("MI_TourniquetGizmo_UseLabel".Translate(Colorize(bodyPart, aggregatedBleedRate).Named(Named.Params.BODYPART)).Colorize(Color.white), patient.Downed
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
                if (KnownResearchProjectDefOf.BasicFirstAid.IsFinished)
                {
                    builder.Options.Add(new FloatMenuOption(failure, null));
                }
                return;
            }

            bool pawnKnowsWhatTheyreDoing = JobDriver_TourniquetBase.PawnKnowsWhatTheyreDoing(selectedPawn);

            using BleedRateByLimbEnumerable bleedRateCache = BleedRateByLimbEnumerable.EvaluateLimbs(patient);
            foreach ((BodyPartRecord bodyPart, float aggregatedBleedRate) in bleedRateCache)
            {
                if (patient.health.hediffSet.hediffs.Any(hediff => hediff.Part == bodyPart && hediff.def == KnownHediffDefOf.TourniquetApplied))
                {
                    // even if you don't know what a tourniquet is, you can still remove it
                    builder.Options.Add(new FloatMenuOption(
                        "MI_TourniquetFloatMenu_RemoveSafelyLabel".Translate(
                            bodyPart.Label.Colorize(Color.red).Named(Named.Params.BODYPART)).Colorize(Color.white),
                    JobDriver_RemoveTourniquetSafely.GetDispatcher(selectedPawn, patient, bodyPart).StartJob));
                    builder.Options.Add(new FloatMenuOption(
                        "MI_TourniquetFloatMenu_RemoveQuicklyLabel".Translate(
                            bodyPart.Label.Colorize(Color.red).Named(Named.Params.BODYPART)).Colorize(Color.white),
                    JobDriver_RemoveTourniquetQuickly.GetDispatcher(selectedPawn, patient, bodyPart).StartJob));
                }
                else if (tourniquet is not null && selectedPawn.Drafted && (bodyPart.def != KnownBodyPartDefOf.Neck || !pawnKnowsWhatTheyreDoing))
                {
                    // applying a tourniquet requires at least knowing what it is
                    if (KnownResearchProjectDefOf.BasicFirstAid.IsFinished)
                    {
                        builder.Options.Add(new FloatMenuOption(
                            "MI_TourniquetFloatMenu_UseLabel".Translate(
                                Colorize(bodyPart, aggregatedBleedRate).Named(Named.Params.BODYPART),
                                patient.Label.Colorize(Color.yellow).Named(Named.Params.PATIENTNAME)).Colorize(Color.white),
                            JobDriver_UseTourniquet.GetDispatcher(selectedPawn, patient, tourniquet, bodyPart).StartJob));
                    }
                }
            }
        }
    }

    private static string Colorize(BodyPartRecord bodyPart, float aggregatedBleedRate) => aggregatedBleedRate switch
    {
        _ when aggregatedBleedRate > MoreInjuriesMod.Settings.MinBleedRateForAutoTourniquet => bodyPart.Label.Colorize(Color.red),
        > 0f => bodyPart.Label.Colorize(Color.yellow),
        _ => bodyPart.Label.Colorize(Color.green)
    };
}