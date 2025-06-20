using MoreInjuries.Roslyn.Metadata.KeyedMembers;
using Verse;

namespace MoreInjuries;

using static MoreInjuriesSettingsDefaults;

[KeyedMembers(Visibility = Visibility.Internal)]
public partial class MoreInjuriesSettings : ModSettings
{
    // logging
    internal bool EnableLogging = ENABLE_LOGGING_DEFAULT;
    internal bool EnableVerboseLogging = ENABLE_VERBOSE_LOGGING_DEFAULT;
    // adrenaline and epinephrine
    internal bool EnableAdrenaline = ENABLE_ADRENALINE_DEFAULT;
    internal float AdrenalineChanceOnDamage = ADRENALINE_CHANCE_ON_DAMAGE_DEFAULT;
    internal float CertainAdrenalineThreshold = CERTAIN_ADRENALINE_THRESHOLD_DEFAULT;
    // blood transfusions
    internal int BloodTransfusionHarvestMinimumSkill = BLOOD_TRANSFUSION_HARVEST_MINIMUM_SKILL_DEFAULT;
    // hydrostatic shock
    internal bool EnableHydrostaticShock = ENABLE_HYDROSTATIC_SHOCK_DEFAULT;
    internal float HydrostaticShockChanceOnDamage = HYDROSTATIC_SHOCK_CHANCE_ON_DAMAGE_DEFAULT;
    // EMP
    internal bool EnableEmpDamageToBionics = ENABLE_EMP_DAMAGE_TO_BIONICS_DEFAULT;
    internal float EmpDamageToBionicsChance = EMP_DAMAGE_TO_BIONICS_CHANCE_DEFAULT;
    // hemorrhagic stroke
    internal bool EnableHemorrhagicStroke = ENABLE_HEMORRHAGIC_STROKE_DEFAULT;
    internal float HemorrhagicStrokeChance = HEMORRHAGIC_STROKE_CHANCE_DEFAULT;
    internal float HemorrhagicStrokeThreshold = HEMORRHAGIC_STROKE_THRESHOLD_DEFAULT;
    // concussions
    internal bool EnableConcussion = ENABLE_CONCUSSION_DEFAULT;
    internal float ConcussionChance = CONCUSSION_CHANCE_DEFAULT;
    internal float ConcussionThreshold = CONCUSSION_THRESHOLD_DEFAULT;
    // choking
    internal bool EnableChoking = ENABLE_CHOKING_DEFAULT;
    internal float ChokingChanceOnDamage = CHOKING_CHANCE_ON_DAMAGE_DEFAULT;
    internal float ChokingMinimumBleedRate = CHOKING_MINIMUM_BLEED_RATE_DEFAULT;
    internal bool EnableChokingSounds = ENABLE_CHOKING_SOUNDS_DEFAULT;
    internal float SuctionDeviceMinimumSuccessRate = SUCTION_DEVICE_MINIMUM_SUCCESS_RATE_DEFAULT;
    // lung collapse
    internal bool EnableLungCollapse = ENABLE_LUNG_COLLAPSE_DEFAULT;
    internal float LungCollapseChanceOnDamage = LUNG_COLLAPSE_CHANCE_ON_DAMAGE_DEFAULT;
    internal float LungCollapseMaxSeverityRoot = LUNG_COLLAPSE_MAX_SEVERITY_ROOT_DEFAULT;
    // spalling
    internal bool EnableSpalling = ENABLE_SPALLING_DEFAULT;
    internal float ArmorHealthSpallingThreshold = ARMOR_HEALTH_SPALLING_THRESHOLD_DEFAULT;
    internal float SpallingChance = SPALLING_CHANCE_DEFAULT;
    // hearing damage
    internal bool EnableBasicHearingDamage = ENABLE_BASIC_HEARING_DAMAGE_DEFAULT;
    internal bool EnableAdvancedHearingDamage = ENABLE_ADVANCED_HEARING_DAMAGE_DEFAULT;
    internal float HearingDamageTemporarySeverityFactor = HEARING_DAMAGE_TEMPORARY_SEVERITY_FACTOR_DEFAULT;
    internal float HearingDamageTemporarySeverityFactorExplosions = HEARING_DAMAGE_TEMPORARY_SEVERITY_FACTOR_EXPLOSIONS_DEFAULT;
    internal bool HearingDamageMayBecomePermanent = HEARING_DAMAGE_MAY_BECOME_PERMANENT_DEFAULT;
    internal float HearingDamagePermanentChanceFactor = HEARING_DAMAGE_PERMANENT_CHANCE_FACTOR_DEFAULT;
    // fractures
    internal bool EnableFractures = ENABLE_FRACTURES_DEFAULT;
    internal float FractureDamageTreshold = FRACTURE_DAMAGE_TRESHOLD_DEFAULT;
    internal float FractureChanceOnDamage = FRACTURE_CHANCE_ON_DAMAGE_DEFAULT;
    internal bool EnableBoneFragmentLacerations = ENABLE_BONE_FRAGMENT_LACERATIONS_DEFAULT;
    internal float SplinteringFractureChance = SPLINTERING_FRACTURE_CHANCE_DEFAULT;
    internal float BoneFragmentLacerationChancePerBodyPart = BONE_FRAGMENT_LACERATION_CHANCE_PER_BODY_PART_DEFAULT;
    internal bool EnableApplySplintJob = ENABLE_APPLY_SPLINT_JOB_DEFAULT;
    // hypovolemic shock and cardiac arrest
    internal bool EnableHypovolemicShock = ENABLE_HYPOVOLEMIC_SHOCK_DEFAULT;
    internal float OrganHypoxiaChance = ORGAN_HYPOXIA_CHANCE_DEFAULT;
    internal float OrganHypoxiaChanceReductionFactor = ORGAN_HYPOXIA_CHANCE_REDUCTION_FACTOR_DEFAULT;
    internal bool EnableCardiacArrestOnHighBloodLoss = ENABLE_CARDIAC_ARREST_ON_HIGH_BLOOD_LOSS_DEFAULT;
    internal float CardiacArrestChanceOnHighBloodLoss = CARDIAC_ARREST_CHANCE_ON_HIGH_BLOOD_LOSS_DEFAULT;
    internal float DefibrillatorMinimumSuccessRate = DEFIBRILLATOR_MINIMUM_SUCCESS_RATE_DEFAULT;
    // neural damage and permanent brain injuries
    internal bool EnableNeuralDamage = ENABLE_NEURAL_DAMAGE_DEFAULT;
    internal float NeuralDamageChanceReductionFactor = NEURAL_DAMAGE_CHANCE_REDUCTION_FACTOR_DEFAULT;
    // tourniquet-related injuries
    internal bool TourniquetsCanCauseGangrene = TOURNIQUETS_CAN_CAUSE_GANGRENE_DEFAULT;
    internal float MeanTimeBetweenGangreneOnTourniquet = MEAN_TIME_BETWEEN_GANGRENE_ON_TOURNIQUET_DEFAULT;
    internal float DryGangreneChance = DRY_GANGRENE_CHANCE_DEFAULT;
    internal float DryGangreneMeanTimeToInfection = DRY_GANGRENE_MEAN_TIME_TO_INFECTION_DEFAULT;
    internal bool EnableTourniquetGizmo = ENABLE_TOURNIQUET_GIZMO_DEFAULT;
    internal float MinBleedRateForAutoTourniquet = MIN_BLEED_RATE_FOR_AUTO_TOURNIQUET_DEFAULT;
    // miscellaneous
    internal bool EnableFireInhalation = ENABLE_FIRE_INHALATION_DEFAULT;
    internal bool BiotechEnableIntegration = BIOTECH_ENABLE_INTEGRATION_DEFAULT;
    internal bool AnomalyEnableConditionsForShamblers = ANOMALY_ENABLE_CONDITIONS_FOR_SHAMBLERS_DEFAULT;
    internal bool EnableParalysis = ENABLE_PARALYSIS_DEFAULT;
    internal float ParalysisDamageTreshold50Percent = PARALYSIS_DAMAGE_TRESHOLD_50_PERCENT_DEFAULT;
    internal float IntestinalSpillingChanceOnDamage = INTESTINAL_SPILLING_CHANCE_ON_DAMAGE_DEFAULT;
    internal float ClosedInternalWouldBleedingModifier = CLOSED_INTERNAL_WOULD_BLEEDING_MODIFIER_DEFAULT;

    public override void ExposeData()
    {
        // logging
        Scribe_Values.Look(ref EnableLogging, nameof(EnableLogging), ENABLE_LOGGING_DEFAULT);
        Scribe_Values.Look(ref EnableVerboseLogging, nameof(EnableVerboseLogging), ENABLE_VERBOSE_LOGGING_DEFAULT);
        // adrenaline
        Scribe_Values.Look(ref EnableAdrenaline, nameof(EnableAdrenaline), ENABLE_ADRENALINE_DEFAULT);
        Scribe_Values.Look(ref AdrenalineChanceOnDamage, nameof(AdrenalineChanceOnDamage), ADRENALINE_CHANCE_ON_DAMAGE_DEFAULT);
        Scribe_Values.Look(ref CertainAdrenalineThreshold, nameof(CertainAdrenalineThreshold), CERTAIN_ADRENALINE_THRESHOLD_DEFAULT);
        // blood transfusions
        Scribe_Values.Look(ref BloodTransfusionHarvestMinimumSkill, nameof(BloodTransfusionHarvestMinimumSkill), BLOOD_TRANSFUSION_HARVEST_MINIMUM_SKILL_DEFAULT);
        // fractures
        Scribe_Values.Look(ref EnableFractures, nameof(EnableFractures), ENABLE_FRACTURES_DEFAULT);
        Scribe_Values.Look(ref FractureDamageTreshold, nameof(FractureDamageTreshold), FRACTURE_DAMAGE_TRESHOLD_DEFAULT);
        Scribe_Values.Look(ref FractureChanceOnDamage, nameof(FractureChanceOnDamage), FRACTURE_CHANCE_ON_DAMAGE_DEFAULT);
        Scribe_Values.Look(ref EnableBoneFragmentLacerations, nameof(EnableBoneFragmentLacerations), ENABLE_BONE_FRAGMENT_LACERATIONS_DEFAULT);
        Scribe_Values.Look(ref SplinteringFractureChance, nameof(SplinteringFractureChance), SPLINTERING_FRACTURE_CHANCE_DEFAULT);
        Scribe_Values.Look(ref BoneFragmentLacerationChancePerBodyPart, nameof(BoneFragmentLacerationChancePerBodyPart), BONE_FRAGMENT_LACERATION_CHANCE_PER_BODY_PART_DEFAULT);
        Scribe_Values.Look(ref EnableApplySplintJob, nameof(EnableApplySplintJob), ENABLE_APPLY_SPLINT_JOB_DEFAULT);
        // hemorrhagic stroke after blunt trauma
        Scribe_Values.Look(ref EnableHemorrhagicStroke, nameof(EnableHemorrhagicStroke), ENABLE_HEMORRHAGIC_STROKE_DEFAULT);
        Scribe_Values.Look(ref HemorrhagicStrokeChance, nameof(HemorrhagicStrokeChance), HEMORRHAGIC_STROKE_CHANCE_DEFAULT);
        Scribe_Values.Look(ref HemorrhagicStrokeThreshold, nameof(HemorrhagicStrokeThreshold), HEMORRHAGIC_STROKE_THRESHOLD_DEFAULT);
        // concussions after blunt trauma
        Scribe_Values.Look(ref EnableConcussion, nameof(EnableConcussion), ENABLE_CONCUSSION_DEFAULT);
        Scribe_Values.Look(ref ConcussionChance, nameof(ConcussionChance), CONCUSSION_CHANCE_DEFAULT);
        Scribe_Values.Look(ref ConcussionThreshold, nameof(ConcussionThreshold), CONCUSSION_THRESHOLD_DEFAULT);
        // choking on blood
        Scribe_Values.Look(ref EnableChoking, nameof(EnableChoking), ENABLE_CHOKING_DEFAULT);
        Scribe_Values.Look(ref EnableChokingSounds, nameof(EnableChokingSounds), ENABLE_CHOKING_SOUNDS_DEFAULT);
        Scribe_Values.Look(ref ChokingMinimumBleedRate, nameof(ChokingMinimumBleedRate), CHOKING_MINIMUM_BLEED_RATE_DEFAULT);
        Scribe_Values.Look(ref ChokingChanceOnDamage, nameof(ChokingChanceOnDamage), CHOKING_CHANCE_ON_DAMAGE_DEFAULT);
        Scribe_Values.Look(ref SuctionDeviceMinimumSuccessRate, nameof(SuctionDeviceMinimumSuccessRate), SUCTION_DEVICE_MINIMUM_SUCCESS_RATE_DEFAULT);
        // spalling
        Scribe_Values.Look(ref EnableSpalling, nameof(EnableSpalling), ENABLE_SPALLING_DEFAULT);
        Scribe_Values.Look(ref ArmorHealthSpallingThreshold, nameof(ArmorHealthSpallingThreshold), ARMOR_HEALTH_SPALLING_THRESHOLD_DEFAULT);
        Scribe_Values.Look(ref SpallingChance, nameof(SpallingChance), SPALLING_CHANCE_DEFAULT);
        // hydrostatic shock
        Scribe_Values.Look(ref EnableHydrostaticShock, nameof(EnableHydrostaticShock), ENABLE_HYDROSTATIC_SHOCK_DEFAULT);
        Scribe_Values.Look(ref HydrostaticShockChanceOnDamage, nameof(HydrostaticShockChanceOnDamage), HYDROSTATIC_SHOCK_CHANCE_ON_DAMAGE_DEFAULT);
        // lung collapse
        Scribe_Values.Look(ref EnableLungCollapse, nameof(EnableLungCollapse), ENABLE_LUNG_COLLAPSE_DEFAULT);
        Scribe_Values.Look(ref LungCollapseChanceOnDamage, nameof(LungCollapseChanceOnDamage), LUNG_COLLAPSE_CHANCE_ON_DAMAGE_DEFAULT);
        Scribe_Values.Look(ref LungCollapseMaxSeverityRoot, nameof(LungCollapseMaxSeverityRoot), LUNG_COLLAPSE_MAX_SEVERITY_ROOT_DEFAULT);
        // EMP
        Scribe_Values.Look(ref EnableEmpDamageToBionics, nameof(EnableEmpDamageToBionics), ENABLE_EMP_DAMAGE_TO_BIONICS_DEFAULT);
        Scribe_Values.Look(ref EmpDamageToBionicsChance, nameof(EmpDamageToBionicsChance), EMP_DAMAGE_TO_BIONICS_CHANCE_DEFAULT);
        // hearing damage
        Scribe_Values.Look(ref EnableBasicHearingDamage, nameof(EnableBasicHearingDamage), ENABLE_BASIC_HEARING_DAMAGE_DEFAULT);
        Scribe_Values.Look(ref EnableAdvancedHearingDamage, nameof(EnableAdvancedHearingDamage), ENABLE_ADVANCED_HEARING_DAMAGE_DEFAULT);
        Scribe_Values.Look(ref HearingDamageTemporarySeverityFactor, nameof(HearingDamageTemporarySeverityFactor), HEARING_DAMAGE_TEMPORARY_SEVERITY_FACTOR_DEFAULT);
        Scribe_Values.Look(ref HearingDamageTemporarySeverityFactorExplosions, nameof(HearingDamageTemporarySeverityFactorExplosions), HEARING_DAMAGE_TEMPORARY_SEVERITY_FACTOR_EXPLOSIONS_DEFAULT);
        Scribe_Values.Look(ref HearingDamageMayBecomePermanent, nameof(HearingDamageMayBecomePermanent), HEARING_DAMAGE_MAY_BECOME_PERMANENT_DEFAULT);
        Scribe_Values.Look(ref HearingDamagePermanentChanceFactor, nameof(HearingDamagePermanentChanceFactor), HEARING_DAMAGE_PERMANENT_CHANCE_FACTOR_DEFAULT);
        // hypovolemic shock
        Scribe_Values.Look(ref EnableHypovolemicShock, nameof(EnableHypovolemicShock), ENABLE_HYPOVOLEMIC_SHOCK_DEFAULT);
        Scribe_Values.Look(ref OrganHypoxiaChance, nameof(OrganHypoxiaChance), ORGAN_HYPOXIA_CHANCE_DEFAULT);
        Scribe_Values.Look(ref OrganHypoxiaChanceReductionFactor, nameof(OrganHypoxiaChanceReductionFactor), ORGAN_HYPOXIA_CHANCE_REDUCTION_FACTOR_DEFAULT);
        Scribe_Values.Look(ref EnableCardiacArrestOnHighBloodLoss, nameof(EnableCardiacArrestOnHighBloodLoss), ENABLE_CARDIAC_ARREST_ON_HIGH_BLOOD_LOSS_DEFAULT);
        Scribe_Values.Look(ref CardiacArrestChanceOnHighBloodLoss, nameof(CardiacArrestChanceOnHighBloodLoss), CARDIAC_ARREST_CHANCE_ON_HIGH_BLOOD_LOSS_DEFAULT);
        Scribe_Values.Look(ref DefibrillatorMinimumSuccessRate, nameof(DefibrillatorMinimumSuccessRate), DEFIBRILLATOR_MINIMUM_SUCCESS_RATE_DEFAULT);
        // neural damage
        Scribe_Values.Look(ref EnableNeuralDamage, nameof(EnableNeuralDamage), ENABLE_NEURAL_DAMAGE_DEFAULT);
        Scribe_Values.Look(ref NeuralDamageChanceReductionFactor, nameof(NeuralDamageChanceReductionFactor), NEURAL_DAMAGE_CHANCE_REDUCTION_FACTOR_DEFAULT);
        // tourniquets
        Scribe_Values.Look(ref TourniquetsCanCauseGangrene, nameof(TourniquetsCanCauseGangrene), TOURNIQUETS_CAN_CAUSE_GANGRENE_DEFAULT);
        Scribe_Values.Look(ref MeanTimeBetweenGangreneOnTourniquet, nameof(MeanTimeBetweenGangreneOnTourniquet), MEAN_TIME_BETWEEN_GANGRENE_ON_TOURNIQUET_DEFAULT);
        Scribe_Values.Look(ref DryGangreneChance, nameof(DryGangreneChance), DRY_GANGRENE_CHANCE_DEFAULT);
        Scribe_Values.Look(ref DryGangreneMeanTimeToInfection, nameof(DryGangreneMeanTimeToInfection), DRY_GANGRENE_MEAN_TIME_TO_INFECTION_DEFAULT);
        Scribe_Values.Look(ref EnableTourniquetGizmo, nameof(EnableTourniquetGizmo), ENABLE_TOURNIQUET_GIZMO_DEFAULT);
        Scribe_Values.Look(ref MinBleedRateForAutoTourniquet, nameof(MinBleedRateForAutoTourniquet), MIN_BLEED_RATE_FOR_AUTO_TOURNIQUET_DEFAULT);
        // miscellaneous
        Scribe_Values.Look(ref ClosedInternalWouldBleedingModifier, nameof(ClosedInternalWouldBleedingModifier), CLOSED_INTERNAL_WOULD_BLEEDING_MODIFIER_DEFAULT);
        Scribe_Values.Look(ref BiotechEnableIntegration, nameof(BiotechEnableIntegration), BIOTECH_ENABLE_INTEGRATION_DEFAULT);
        Scribe_Values.Look(ref AnomalyEnableConditionsForShamblers, nameof(AnomalyEnableConditionsForShamblers), ANOMALY_ENABLE_CONDITIONS_FOR_SHAMBLERS_DEFAULT);
        Scribe_Values.Look(ref EnableFireInhalation, nameof(EnableFireInhalation), ENABLE_FIRE_INHALATION_DEFAULT);
        Scribe_Values.Look(ref EnableParalysis, nameof(EnableParalysis), ENABLE_PARALYSIS_DEFAULT);
        Scribe_Values.Look(ref ParalysisDamageTreshold50Percent, nameof(ParalysisDamageTreshold50Percent), PARALYSIS_DAMAGE_TRESHOLD_50_PERCENT_DEFAULT);
        Scribe_Values.Look(ref IntestinalSpillingChanceOnDamage, nameof(IntestinalSpillingChanceOnDamage), INTESTINAL_SPILLING_CHANCE_ON_DAMAGE_DEFAULT);

        base.ExposeData();
    }
}

