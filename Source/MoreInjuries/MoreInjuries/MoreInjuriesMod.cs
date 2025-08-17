using System.Linq;
using RimWorld;
using HarmonyLib;
using Verse;
using UnityEngine;
using MoreInjuries.Initialization;
using System.Collections.Generic;
using MoreInjuries.Localization;
using static MoreInjuries.MoreInjuriesSettings.Defaults;

namespace MoreInjuries;

public class MoreInjuriesMod : Mod
{
    private static bool? s_combatExtendedLoaded = null;

    public static MoreInjuriesSettings Settings { get; private set; } = null!;

    internal static bool CombatExtendedLoaded => 
        s_combatExtendedLoaded ??= LoadedModManager.RunningModsListForReading.Any(static mod => mod.PackageIdPlayerFacing?.Equals("CETeam.CombatExtended") is true);

    public MoreInjuriesMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<MoreInjuriesSettings>();
        Harmony harmony = new("Th3Fr3d.MoreInjuries");
        harmony.PatchAll();
    }

    private Vector2 _scrollPosition;
    private const float MIN_CONTENT_HEIGHT = 512f;
    private float _knownContentHeight = MIN_CONTENT_HEIGHT;
    private bool _requiresScrolling = false;
    private bool _hasMap = false;

    public override void DoSettingsWindowContents(Rect canvas)
    {
        if (Settings is null)
        {
            Logger.Error("Settings was null!");
            throw new ArgumentNullException(nameof(Settings));
        }
        float scrollbarMargin = _requiresScrolling ? 25f : 0f;
        Listing_Standard list = new();
        Rect content = new(canvas.x, canvas.y, canvas.width - scrollbarMargin, _knownContentHeight);
        Rect view = new(canvas.x, canvas.y, canvas.width, canvas.height);
        if (_requiresScrolling)
        {
            Widgets.BeginScrollView(view, ref _scrollPosition, content);
        }
        list.Begin(content);
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_General".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_General_LoggingLabel".Translate(ENABLE_LOGGING_DEFAULT.NamedDefault()), ref Settings.EnableLogging);
        list.CheckboxLabeled("MI_Settings_General_LoggingVerboseLabel".Translate(ENABLE_VERBOSE_LOGGING_DEFAULT.NamedDefault()), ref Settings.EnableVerboseLogging);
        if (Find.CurrentMap is not null)
        {
            if (!_hasMap)
            {
                _hasMap = true;
                _knownContentHeight = MIN_CONTENT_HEIGHT;
            }
            if (list.ButtonText("MI_Settings_General_DebugAction_FixBionicsLabel".Translate()))
            {
                IEnumerable<Pawn> humans = Find.CurrentMap.mapPawns.AllPawns.Where(static pawn => pawn.def == ThingDefOf.Human);
                foreach (Pawn human in humans)
                {
                    FixMisplacedBionicsModExtension.FixPawn(human);
                }
            }
        }
        else if (_hasMap)
        {
            _hasMap = false;
            _knownContentHeight = MIN_CONTENT_HEIGHT;
        }
        list.GapLine();
        list.Gap();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features".Translate());
        Text.Font = GameFont.Small;
        // fractures
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_Fractures".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_Fractures_EnableLabel".Translate(ENABLE_FRACTURES_DEFAULT.NamedDefault()), ref Settings.EnableFractures,
            "MI_Settings_Features_Fractures_EnableTooltip".Translate());
        list.Label("MI_Settings_Features_Fractures_DamageThresholdLabel".Translate(Settings.FractureDamageTreshold.NamedValue(), FRACTURE_DAMAGE_TRESHOLD_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Fractures_DamageThresholdTooltip".Translate());
        Settings.FractureDamageTreshold = (float)Math.Ceiling(list.Slider(Mathf.Clamp(Settings.FractureDamageTreshold, 1f, 20f), 1f, 20f));
        list.Label("MI_Settings_Features_Fractures_FractureChanceLabel".Translate(Settings.FractureChanceOnDamage.NamedValue(), FRACTURE_CHANCE_ON_DAMAGE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Fractures_FractureChanceTooltip".Translate());
        Settings.FractureChanceOnDamage = (float)Math.Round(list.Slider(Settings.FractureChanceOnDamage, 0f, 1f), 2);
        list.CheckboxLabeled("MI_Settings_Features_Fractures_EnableLacerationsLabel".Translate(ENABLE_BONE_FRAGMENT_LACERATIONS_DEFAULT.NamedDefault()), ref Settings.EnableBoneFragmentLacerations,
            "MI_Settings_Features_Fractures_EnableLacerationsTooltip".Translate());
        list.Label("MI_Settings_Features_Fractures_LacerationChanceLabel".Translate(Settings.SplinteringFractureChance.NamedValue(), SPLINTERING_FRACTURE_CHANCE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Fractures_LacerationChanceTooltip".Translate());
        Settings.SplinteringFractureChance = (float)Math.Round(list.Slider(Settings.SplinteringFractureChance, 0f, 1f), 2);
        list.Label("MI_Settings_Features_Fractures_LacerationInjuryChanceLabel".Translate(Settings.BoneFragmentLacerationChancePerBodyPart.NamedValue(), BONE_FRAGMENT_LACERATION_CHANCE_PER_BODY_PART_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Fractures_LacerationInjuryChanceTooltip".Translate());
        Settings.BoneFragmentLacerationChancePerBodyPart = (float)Math.Round(list.Slider(Settings.BoneFragmentLacerationChancePerBodyPart, 0f, 1f), 2);
        list.CheckboxLabeled("MI_Settings_Features_Fractures_ApplySplintJobLabel".Translate(ENABLE_APPLY_SPLINT_JOB_DEFAULT.NamedDefault()), ref Settings.EnableApplySplintJob,
            "MI_Settings_Features_Fractures_ApplySplintJobTooltip".Translate());
        // respiratory conditions
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_Respiratory".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_Respiratory_EnableChokingLabel".Translate(ENABLE_CHOKING_DEFAULT.NamedDefault()), ref Settings.EnableChoking,
            "MI_Settings_Features_Respiratory_EnableChokingTooltip".Translate());
        list.CheckboxLabeled("MI_Settings_Features_Respiratory_EnableChokingSoundLabel".Translate(ENABLE_CHOKING_SOUNDS_DEFAULT.NamedDefault()), ref Settings.EnableChokingSounds,
            "MI_Settings_Features_Respiratory_EnableChokingSoundTooltip".Translate());
        list.Label("MI_Settings_Features_Respiratory_ChokingMinBleedRateLabel".Translate(Settings.ChokingMinimumBleedRate.NamedValue(), CHOKING_MINIMUM_BLEED_RATE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Respiratory_ChokingMinBleedRateTooltip".Translate());
        Settings.ChokingMinimumBleedRate = (float)Math.Round(list.Slider(Settings.ChokingMinimumBleedRate, 0f, 1f), 2);
        list.Label("MI_Settings_Features_Respiratory_ChokingChanceLabel".Translate(Settings.ChokingChanceOnDamage.NamedValue(), CHOKING_CHANCE_ON_DAMAGE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Respiratory_ChokingChanceTooltip".Translate());
        Settings.ChokingChanceOnDamage = (float)Math.Round(list.Slider(Settings.ChokingChanceOnDamage, 0f, 1f), 2);
        list.Label("MI_Settings_Features_Respiratory_ChokingSuctionDeviceMinSuccessRateLabel".Translate(Settings.SuctionDeviceMinimumSuccessRate.NamedValue(), SUCTION_DEVICE_MINIMUM_SUCCESS_RATE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Respiratory_ChokingSuctionDeviceMinSuccessRateTooltip".Translate());
        Settings.SuctionDeviceMinimumSuccessRate = (float)Math.Round(list.Slider(Settings.SuctionDeviceMinimumSuccessRate, 0f, 1f), 2);
        list.CheckboxLabeled("MI_Settings_Features_Respiratory_EnableInhalationLabel".Translate(ENABLE_FIRE_INHALATION_DEFAULT.NamedDefault()), ref Settings.EnableFireInhalation,
            "MI_Settings_Features_Respiratory_EnableInhalationTooltip".Translate());
        list.CheckboxLabeled("MI_Settings_Features_Respiratory_EnableLungCollapseLabel".Translate(ENABLE_LUNG_COLLAPSE_DEFAULT.NamedDefault()), ref Settings.EnableLungCollapse,
            "MI_Settings_Features_Respiratory_EnableLungCollapseTooltip".Translate());
        list.Label("MI_Settings_Features_Respiratory_LungCollapseChanceLabel".Translate(Settings.LungCollapseChanceOnThermobaricDamage.NamedValue(), LUNG_COLLAPSE_CHANCE_ON_THERMOBARIC_DAMAGE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Respiratory_LungCollapseChanceTooltip".Translate());
        Settings.LungCollapseChanceOnThermobaricDamage = (float)Math.Round(list.Slider(Settings.LungCollapseChanceOnThermobaricDamage, 0f, 1f), 2);
        list.Label("MI_Settings_Features_Respiratory_LungCollapseOnPerforationChanceLabel".Translate(Settings.LungCollapseChanceOnPerforatingDamage.NamedValue(), LUNG_COLLAPSE_CHANCE_ON_PERFORATING_DAMAGE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Respiratory_LungCollapseOnPerforationChanceLabelTooltip".Translate());
        Settings.LungCollapseChanceOnPerforatingDamage = (float)Math.Round(list.Slider(Settings.LungCollapseChanceOnPerforatingDamage, 0f, 1f), 2);
        list.Label("MI_Settings_Features_Respiratory_LungCollapseMaxSeverityRootLabel".Translate(Settings.LungCollapseMaxSeverityRoot.NamedValue(), LUNG_COLLAPSE_MAX_SEVERITY_ROOT_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Respiratory_LungCollapseMaxSeverityRootTooltip".Translate());
        Settings.LungCollapseMaxSeverityRoot = (float)Math.Round(list.Slider(Mathf.Clamp(Settings.LungCollapseMaxSeverityRoot, 0.1f, 1f), 0.1f, 1f), 2);
        // Spalling
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_Spalling".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_Spalling_EnableLabel".Translate(ENABLE_SPALLING_DEFAULT.NamedDefault()), ref Settings.EnableSpalling,
            "MI_Settings_Features_Spalling_EnableTooltip".Translate());
        list.Label("MI_Settings_Features_Spalling_DamageThresholdLabel".Translate(Settings.ArmorHealthSpallingThreshold.NamedValue(), ARMOR_HEALTH_SPALLING_THRESHOLD_DEFAULT.NamedDefault()));
        Settings.ArmorHealthSpallingThreshold = (float)Math.Round(list.Slider(Mathf.Clamp(Settings.ArmorHealthSpallingThreshold, 0.1f, 1f), 0.1f, 1f), 2);
        list.Label("MI_Settings_Features_Spalling_ChanceLabel".Translate(Settings.SpallingChance.NamedValue(), SPALLING_CHANCE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Spalling_ChanceTooltip".Translate());
        Settings.SpallingChance = (float)Math.Round(list.Slider(Settings.SpallingChance, 0f, 1f), 2);
        // hypovolemic shock, cardiac arrest, and the trauma triad of death
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_HypovolemicShock".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_HypovolemicShock_EnableLabel".Translate(ENABLE_HYPOVOLEMIC_SHOCK_DEFAULT.NamedDefault()), ref Settings.EnableHypovolemicShock,
            "MI_Settings_Features_HypovolemicShock_EnableTooltip".Translate());
        list.Label("MI_Settings_Features_HypovolemicShock_HypoxiaChanceLabel".Translate(Settings.OrganHypoxiaChance.NamedValue(), ORGAN_HYPOXIA_CHANCE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_HypovolemicShock_HypoxiaChanceTooltip".Translate());
        Settings.OrganHypoxiaChance = (float)Math.Round(list.Slider(Settings.OrganHypoxiaChance, 0f, 1f), 2);
        list.Label("MI_Settings_Features_HypovolemicShock_HypoxiaReductionFactorLabel".Translate(Settings.OrganHypoxiaChanceReductionFactor.NamedValue(), ORGAN_HYPOXIA_CHANCE_REDUCTION_FACTOR_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_HypovolemicShock_HypoxiaReductionFactorTooltip".Translate());
        Settings.OrganHypoxiaChanceReductionFactor = (float)Math.Round(list.Slider(Settings.OrganHypoxiaChanceReductionFactor, 0f, 1f), 2);
        list.CheckboxLabeled("MI_Settings_Features_HypovolemicShock_EnableCardiacArrestLabel".Translate(ENABLE_CARDIAC_ARREST_ON_HIGH_BLOOD_LOSS_DEFAULT.NamedDefault()), ref Settings.EnableCardiacArrestOnHighBloodLoss,
            "MI_Settings_Features_HypovolemicShock_EnableCardiacArrestTooltip".Translate());
        list.Label("MI_Settings_Features_HypovolemicShock_CardiacArrestChanceLabel".Translate(Settings.CardiacArrestChanceOnHighBloodLoss.NamedValue(), CARDIAC_ARREST_CHANCE_ON_HIGH_BLOOD_LOSS_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_HypovolemicShock_CardiacArrestChanceTooltip".Translate());
        Settings.CardiacArrestChanceOnHighBloodLoss = (float)Math.Round(list.Slider(Settings.CardiacArrestChanceOnHighBloodLoss, 0f, 1f), 2);
        list.Label("MI_Settings_Features_HypovolemicShock_CardiacArrestDefibrillationChanceLabel".Translate(Settings.DefibrillatorMinimumSuccessRate.NamedValue(), DEFIBRILLATOR_MINIMUM_SUCCESS_RATE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_HypovolemicShock_CardiacArrestDefibrillationChanceTooltip".Translate());
        Settings.DefibrillatorMinimumSuccessRate = (float)Math.Round(list.Slider(Settings.DefibrillatorMinimumSuccessRate, 0f, 1f), 2);
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_LethalTriad".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_LethalTriad_EnableLabel".Translate(ENABLE_ADVANCED_TRAUMA_SIMULATION_DEFAULT.NamedDefault()), ref Settings.EnableAdvancedTraumaSimulation,
            "MI_Settings_Features_LethalTriad_EnableTooltip".Translate());
        list.Label("MI_Settings_Features_LethalTriad_HypoxiaAcidosisConversionFactorLabel".Translate(Math.Round(Settings.HypoxiaAcidosisConversionFactor * 100f, 2).NamedValue(), Math.Round(HYPOXIA_ACIDOSIS_CONVERSION_FACTOR_DEFAULT * 100f, 2).NamedDefault()), -1,
            "MI_Settings_Features_LethalTriad_HypoxiaAcidosisConversionFactorTooltip".Translate());
        Settings.HypoxiaAcidosisConversionFactor = (float)Math.Round(list.Slider((float)Math.Round(Settings.HypoxiaAcidosisConversionFactor * 100f, 2), 0f, 5f) / 100f, 4);
        // neural damage
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_NeuralDamage".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_NeuralDamage_EnableLabel".Translate(ENABLE_NEURAL_DAMAGE_DEFAULT.NamedDefault()), ref Settings.EnableNeuralDamage,
            "MI_Settings_Features_NeuralDamage_EnableTooltip".Translate());
        list.Label("MI_Settings_Features_NeuralDamage_TreatmentReductionFactorLabel".Translate(Settings.NeuralDamageChanceReductionFactor.NamedValue(), NEURAL_DAMAGE_CHANCE_REDUCTION_FACTOR_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_NeuralDamage_TreatmentReductionFactorTooltip".Translate());
        Settings.NeuralDamageChanceReductionFactor = (float)Math.Round(list.Slider(Settings.NeuralDamageChanceReductionFactor, 0.01f, 1f), 2);
        list.CheckboxLabeled("MI_Settings_Features_NeuralDamage_EnablePersonalityShiftLabel".Translate(NEURAL_DAMAGE_ENABLE_PERSONALITY_SHIFT_DEFAULT.NamedDefault()), ref Settings.NeuralDamageEnablePersonalityShift,
            "MI_Settings_Features_NeuralDamage_EnablePersonalityShiftTooltip".Translate());
        // concussion after blunt trauma
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_Concussion".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_Concussion_EnableLabel".Translate(ENABLE_CONCUSSION_DEFAULT.NamedDefault()), ref Settings.EnableConcussion,
            "MI_Settings_Features_Concussion_EnableTooltip".Translate());
        list.Label("MI_Settings_Features_Concussion_DamageThresholdLabel".Translate(Settings.ConcussionThreshold.NamedValue(), CONCUSSION_THRESHOLD_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Concussion_DamageThresholdTooltip".Translate());
        Settings.ConcussionThreshold = (float)Math.Ceiling(list.Slider(Mathf.Clamp(Settings.ConcussionThreshold, 1f, 50f), 1f, 50f));
        list.Label("MI_Settings_Features_Concussion_ConcussionChanceLabel".Translate(Settings.ConcussionChance.NamedValue(), CONCUSSION_CHANCE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Concussion_ConcussionChanceTooltip".Translate());
        Settings.ConcussionChance = (float)Math.Round(list.Slider(Settings.ConcussionChance, 0f, 1f), 2);
        // hemorrhagic stroke after blunt trauma
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_HemorrhagicStroke".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_HemorrhagicStroke_EnableLabel".Translate(ENABLE_HEMORRHAGIC_STROKE_DEFAULT.NamedDefault()), ref Settings.EnableHemorrhagicStroke,
            "MI_Settings_Features_HemorrhagicStroke_EnableTooltip".Translate());
        list.Label("MI_Settings_Features_HemorrhagicStroke_DamageThresholdLabel".Translate(Settings.HemorrhagicStrokeThreshold.NamedValue(), HEMORRHAGIC_STROKE_THRESHOLD_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_HemorrhagicStroke_DamageThresholdTooltip".Translate());
        Settings.HemorrhagicStrokeThreshold = (float)Math.Ceiling(list.Slider(Mathf.Clamp(Settings.HemorrhagicStrokeThreshold, 1f, 50f), 1f, 50f));
        list.Label("MI_Settings_Features_HemorrhagicStroke_StrokeChanceLabel".Translate(Settings.HemorrhagicStrokeChance.NamedValue(), HEMORRHAGIC_STROKE_CHANCE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_HemorrhagicStroke_StrokeChanceTooltip".Translate());
        Settings.HemorrhagicStrokeChance = (float)Math.Round(list.Slider(Settings.HemorrhagicStrokeChance, 0f, 1f), 2);
        // EMP damage to bionics
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_EmpDamage".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_EmpDamage_EnableLabel".Translate(ENABLE_EMP_DAMAGE_TO_BIONICS_DEFAULT.NamedDefault()), ref Settings.EnableEmpDamageToBionics,
            "MI_Settings_Features_EmpDamage_EnableTooltip".Translate());
        list.Label("MI_Settings_Features_EmpDamage_ChanceLabel".Translate(Settings.EmpDamageToBionicsChance.NamedValue(), EMP_DAMAGE_TO_BIONICS_CHANCE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_EmpDamage_ChanceTooltip".Translate());
        Settings.EmpDamageToBionicsChance = (float)Math.Round(list.Slider(Settings.EmpDamageToBionicsChance, 0f, 1f), 2);
        // adrenaline
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_Adrenaline".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_Adrenaline_EnableLabel".Translate(ENABLE_ADRENALINE_DEFAULT.NamedDefault()), ref Settings.EnableAdrenaline,
            "MI_Settings_Features_Adrenaline_EnableTooltip".Translate());
        list.Label("MI_Settings_Features_Adrenaline_DamageThresholdLabel".Translate(Settings.CertainAdrenalineThreshold.NamedValue(), CERTAIN_ADRENALINE_THRESHOLD_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Adrenaline_DamageThresholdTooltip".Translate());
        Settings.CertainAdrenalineThreshold = (float)Math.Round(list.Slider(Mathf.Clamp(Settings.CertainAdrenalineThreshold, 1f, 50f), 1f, 50f));
        list.Label("MI_Settings_Features_Adrenaline_ChanceLabel".Translate(Settings.AdrenalineChanceOnDamage.NamedValue(), ADRENALINE_CHANCE_ON_DAMAGE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Adrenaline_ChanceTooltip".Translate());
        Settings.AdrenalineChanceOnDamage = (float)Math.Round(list.Slider(Settings.AdrenalineChanceOnDamage, 0f, 1f), 2);
        // hydrostatic shock (controversial)
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_HydrostaticShock".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_HydrostaticShock_EnableLabel".Translate(ENABLE_HYDROSTATIC_SHOCK_DEFAULT.NamedDefault()), ref Settings.EnableHydrostaticShock,
            "MI_Settings_Features_HydrostaticShock_EnableTooltip".Translate());
        list.Label("MI_Settings_Features_HydrostaticShock_ChanceLabel".Translate(Settings.HydrostaticShockChanceOnDamage.NamedValue(), HYDROSTATIC_SHOCK_CHANCE_ON_DAMAGE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_HydrostaticShock_ChanceTooltip".Translate());
        Settings.HydrostaticShockChanceOnDamage = (float)Math.Round(list.Slider(Settings.HydrostaticShockChanceOnDamage, 0f, 1f), 2);
        // tourniquets and gangrene
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_Tourniquets".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_Tourniquets_EnableLabel".Translate(TOURNIQUETS_CAN_CAUSE_GANGRENE_DEFAULT.NamedDefault()), ref Settings.TourniquetsCanCauseGangrene,
            "MI_Settings_Features_Tourniquets_EnableTooltip".Translate());
        list.Label("MI_Settings_Features_Tourniquets_GangreneMeanTimeLabel".Translate(
            Named.Keys.Format_TimeHours.Translate(Math.Round(Settings.MeanTimeBetweenGangreneOnTourniquet / 2500f, 1).Named(Named.Params.HOURS)).NamedValue(),
            Named.Keys.Format_TimeHours.Translate(Math.Round(MEAN_TIME_BETWEEN_GANGRENE_ON_TOURNIQUET_DEFAULT / 2500f, 1).Named(Named.Params.HOURS)).NamedDefault()), -1,
            "MI_Settings_Features_Tourniquets_GangreneMeanTimeTooltip".Translate());
        Settings.MeanTimeBetweenGangreneOnTourniquet = Mathf.Floor(list.Slider((float)Math.Round(Settings.MeanTimeBetweenGangreneOnTourniquet / 2500f, 1), 0.1f, 24f) * 2500f);
        list.Label("MI_Settings_Features_Tourniquets_GangreneDryChanceLabel".Translate(Settings.DryGangreneChance.NamedValue(), DRY_GANGRENE_CHANCE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Tourniquets_GangreneDryChanceTooltip".Translate());
        Settings.DryGangreneChance = (float)Math.Round(list.Slider(Settings.DryGangreneChance, 0f, 1f), 2);
        list.Label("MI_Settings_Features_Tourniquets_WetGangreneMeanTimeLabel".Translate(
            Named.Keys.Format_TimeDays.Translate(Math.Round(Settings.DryGangreneMeanTimeToInfection / 60_000f, 1).Named(Named.Params.DAYS)).NamedValue(),
            Named.Keys.Format_TimeDays.Translate(Math.Round(DRY_GANGRENE_MEAN_TIME_TO_INFECTION_DEFAULT / 60_000f, 1).Named(Named.Params.DAYS)).NamedDefault()), -1,
            "MI_Settings_Features_Tourniquets_WetGangreneMeanTimeTooltip".Translate());
        Settings.DryGangreneMeanTimeToInfection = Mathf.Floor(list.Slider((float)Math.Round(Settings.DryGangreneMeanTimeToInfection / 60_000f, 1), 0.1f, 15f) * 60_000f);
        list.CheckboxLabeled("MI_Settings_Features_Tourniquets_ShowGizmoLabel".Translate(ENABLE_TOURNIQUET_GIZMO_DEFAULT.NamedDefault()), ref Settings.EnableTourniquetGizmo,
            "MI_Settings_Features_Tourniquets_ShowGizmoTooltip".Translate());
        list.Label("MI_Settings_Features_Tourniquets_MinBleedRateForAutoTourniquetLabel".Translate(Settings.MinBleedRateForAutoTourniquet.NamedValue(), MIN_BLEED_RATE_FOR_AUTO_TOURNIQUET_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Tourniquets_MinBleedRateForAutoTourniquetTooltip".Translate());
        Settings.MinBleedRateForAutoTourniquet = (float)Math.Round(Mathf.Clamp(list.Slider(Settings.MinBleedRateForAutoTourniquet, 0.05f, 5f), 0.05f, 5f), 2);
        // hearing damage
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_HearingDamage".Translate());
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("MI_Settings_Features_HearingDamage_EnableLabel".Translate(ENABLE_BASIC_HEARING_DAMAGE_DEFAULT.NamedDefault()), ref Settings.EnableBasicHearingDamage,
            "MI_Settings_Features_HearingDamage_EnableTooltip".Translate());
        bool advancedHearingDamage = Settings.EnableAdvancedHearingDamage;
        list.CheckboxLabeled("MI_Settings_Features_HearingDamage_EnableAdvancedLabel".Translate(ENABLE_ADVANCED_HEARING_DAMAGE_DEFAULT.NamedDefault()), ref advancedHearingDamage,
            "MI_Settings_Features_HearingDamage_EnableAdvancedTooltip".Translate());
        Settings.EnableAdvancedHearingDamage = Settings.EnableBasicHearingDamage && advancedHearingDamage;
        list.Label("MI_Settings_Features_HearingDamage_GunshotFactorLabel".Translate(Settings.HearingDamageTemporarySeverityFactor.NamedValue(), HEARING_DAMAGE_TEMPORARY_SEVERITY_FACTOR_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_HearingDamage_GunshotFactorTooltip".Translate());
        Settings.HearingDamageTemporarySeverityFactor = (float)Math.Round(list.Slider(Settings.HearingDamageTemporarySeverityFactor, 0.01f, 1f), 2);
        list.Label("MI_Settings_Features_HearingDamage_ExplosionFactorLabel".Translate(Settings.HearingDamageTemporarySeverityFactorExplosions.NamedValue(), HEARING_DAMAGE_TEMPORARY_SEVERITY_FACTOR_EXPLOSIONS_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_HearingDamage_ExplosionFactorTooltip".Translate());
        Settings.HearingDamageTemporarySeverityFactorExplosions = (float)Math.Round(list.Slider(Settings.HearingDamageTemporarySeverityFactorExplosions, 0.01f, 2f), 2);
        list.CheckboxLabeled("MI_Settings_Features_HearingDamage_PermanentEnableLabel".Translate(HEARING_DAMAGE_MAY_BECOME_PERMANENT_DEFAULT.NamedDefault()), ref Settings.HearingDamageMayBecomePermanent,
            "MI_Settings_Features_HearingDamage_PermanentEnableTooltip".Translate());
        list.Label("MI_Settings_Features_HearingDamage_PermanentMaxChanceLabel".Translate(Settings.HearingDamagePermanentChanceFactor.NamedValue(), HEARING_DAMAGE_PERMANENT_CHANCE_FACTOR_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_HearingDamage_PermanentMaxChanceTooltip".Translate());
        Settings.HearingDamagePermanentChanceFactor = (float)Math.Round(list.Slider(Settings.HearingDamagePermanentChanceFactor, 0.01f, 1f), 2);
        // miscellaneous
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("MI_Settings_Features_Misc".Translate());
        Text.Font = GameFont.Small;
        if (ModLister.BiotechInstalled)
        {
            list.CheckboxLabeled("MI_Settings_Features_Misc_IntegrationBiotechLabel".Translate(BIOTECH_ENABLE_INTEGRATION_DEFAULT.NamedDefault()), ref Settings.BiotechEnableIntegration,
                "MI_Settings_Features_Misc_IntegrationBiotechTooltip".Translate());
        }
        if (ModLister.AnomalyInstalled)
        {
            list.CheckboxLabeled("MI_Settings_Features_Misc_IntegrationAnomalyLabel".Translate(ANOMALY_ENABLE_CONDITIONS_FOR_SHAMBLERS_DEFAULT.NamedDefault()), ref Settings.AnomalyEnableConditionsForShamblers,
                "MI_Settings_Features_Misc_IntegrationAnomalyTooltip".Translate());
        }
        list.Label("MI_Settings_Features_Misc_BleedingReductionFactorEnclosedLabel".Translate(Settings.ClosedInternalWouldBleedingModifier.NamedValue(), CLOSED_INTERNAL_WOULD_BLEEDING_MODIFIER_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Misc_BleedingReductionFactorEnclosedTooltip".Translate());
        Settings.ClosedInternalWouldBleedingModifier = (float)Math.Round(list.Slider(Settings.ClosedInternalWouldBleedingModifier, 0f, 1f), 2);
        list.CheckboxLabeled("MI_Settings_Features_Misc_ParalysisEnableLabel".Translate(ENABLE_PARALYSIS_DEFAULT.NamedDefault()), ref Settings.EnableParalysis,
            "MI_Settings_Features_Misc_ParalysisEnableTooltip".Translate());
        list.Label("MI_Settings_Features_Misc_ParalysisDamageThresholdLabel".Translate(Settings.ParalysisDamageTreshold50Percent.NamedValue(), PARALYSIS_DAMAGE_TRESHOLD_50_PERCENT_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Misc_ParalysisDamageThresholdTooltip".Translate());
        Settings.ParalysisDamageTreshold50Percent = (float)Math.Round(list.Slider(Mathf.Clamp(Settings.ParalysisDamageTreshold50Percent, 1f, 20f), 1f, 20f));
        list.Label("MI_Settings_Features_Misc_IntestinalSpillChanceLabel".Translate(Settings.IntestinalSpillingChanceOnDamage.NamedValue(), INTESTINAL_SPILLING_CHANCE_ON_DAMAGE_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Misc_IntestinalSpillChanceTooltip".Translate());
        Settings.IntestinalSpillingChanceOnDamage = (float)Math.Round(list.Slider(Settings.IntestinalSpillingChanceOnDamage, 0f, 1f), 2);
        list.Label("MI_Settings_Features_Misc_BloodHarvestMinSkillLabel".Translate(Settings.BloodTransfusionHarvestMinimumSkill.NamedValue(), BLOOD_TRANSFUSION_HARVEST_MINIMUM_SKILL_DEFAULT.NamedDefault()), -1,
            "MI_Settings_Features_Misc_BloodHarvestMinSkillTooltip".Translate());
        Settings.BloodTransfusionHarvestMinimumSkill = (int)Math.Round(list.Slider(Settings.BloodTransfusionHarvestMinimumSkill, 0, 20));

        // FIXME: this isn't great, but it works
        // we basically do an initial draw with a rather small height, and see if it's enough
        // - if it is, we remember the exact height of the content and resize the window to fit
        // - otherwise CurHeight will be smaller than a known epsilon, so we start at the epsilon and double it with each draw until it's big enough
        // - once we have enough space, we clamp the height to the known content height
        // if the content is too small to fit everything, CurHeight will be 23.something, (don't ask me why)
        if (list.CurHeight < MIN_CONTENT_HEIGHT)
        {
            // so on each draw, we double the known content height until it's big enough
            // yes, we could also just set it to an unreasonably high value, but that seems a bit wasteful
            // we should arrive at the correct height in O(log n) iterations anyway
            _knownContentHeight = 1000000f;
        }
        else
        {
            // when we have enough space, we remember the height
            _knownContentHeight = list.CurHeight;
        }
        list.End();
        bool requiresScrolling = _knownContentHeight > canvas.height;
        if (_requiresScrolling)
        {
            Widgets.EndScrollView();
        }
        _requiresScrolling = requiresScrolling;
        base.DoSettingsWindowContents(canvas);
    }

    public override string SettingsCategory() => "More Injuries (Continued)";
}

file static class NamedExtensions
{
    /// <summary>
    /// Returns <paramref name="obj"/> as a named argument called <c>VALUE</c>.
    /// </summary>
    public static NamedArgument NamedValue<T>(this T obj) =>
        obj.ToStringSafe().Named(Named.Params.VALUE);

    /// <summary>
    /// Returns <paramref name="obj"/> as a named argument called <c>VALUE_DEFAULT</c>.
    /// </summary>
    public static NamedArgument NamedDefault<T>(this T obj) =>
        obj.ToStringSafe().Named(Named.Params.DEFAULT);
}