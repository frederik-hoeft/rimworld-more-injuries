namespace MoreInjuries;

internal static class MoreInjuriesSettingsDefaults
{
    // logging
    internal const bool ENABLE_LOGGING_DEFAULT = false;
    internal const bool ENABLE_VERBOSE_LOGGING_DEFAULT = false;
    // adrenaline and epinephrine
    internal const bool ENABLE_ADRENALINE_DEFAULT = true;
    internal const float ADRENALINE_CHANCE_ON_DAMAGE_DEFAULT = 0.75f;
    internal const float CERTAIN_ADRENALINE_THRESHOLD_DEFAULT = 15f;
    // hearing damage
    internal const bool ENABLE_BASIC_HEARING_DAMAGE_DEFAULT = true;
    internal const bool ENABLE_ADVANCED_HEARING_DAMAGE_DEFAULT = false;
    // fractures
    internal const bool ENABLE_FRACTURES_DEFAULT = true;
    internal const float FRACTURE_CHANCE_ON_DAMAGE_DEFAULT = 0.25f;
    internal const float FRACTURE_DAMAGE_TRESHOLD_DEFAULT = 10f;
    internal const bool ENABLE_BONE_FRAGMENT_LACERATIONS_DEFAULT = true;
    internal const float SPLINTERING_FRACTURE_CHANCE_DEFAULT = 0.5f;
    internal const float BONE_FRAGMENT_LACERATION_CHANCE_PER_BODY_PART_DEFAULT = 0.5f;
    // hypovolemic shock and cardiac arrest
    internal const bool ENABLE_HYPOVOLEMIC_SHOCK_DEFAULT = true;
    internal const float ORGAN_HYPOXIA_CHANCE_DEFAULT = 0.35f;
    internal const float ORGAN_HYPOXIA_CHANCE_REDUCTION_FACTOR_DEFAULT = 0.5f;
    internal const bool ENABLE_CARDIAC_ARREST_ON_HIGH_BLOOD_LOSS_DEFAULT = true;
    internal const float CARDIAC_ARREST_CHANCE_ON_HIGH_BLOOD_LOSS_DEFAULT = 0.05f;
    internal const float DEFIBRILLATOR_MINIMUM_SUCCESS_RATE_DEFAULT = 0.5f;
    // choking
    internal const bool ENABLE_CHOKING_DEFAULT = true;
    internal const bool ENABLE_CHOKING_SOUNDS_DEFAULT = true;
    internal const float CHOKING_CHANCE_ON_DAMAGE_DEFAULT = 0.75f;
    internal const float CHOKING_MINIMUM_BLEED_RATE_DEFAULT = 0.2f;
    internal const float SUCTION_DEVICE_MINIMUM_SUCCESS_RATE_DEFAULT = 0.25f;
    // concussions
    internal const bool ENABLE_CONCUSSION_DEFAULT = true;
    internal const float CONCUSSION_CHANCE_DEFAULT = 0.75f;
    internal const float CONCUSSION_THRESHOLD_DEFAULT = 6f;
    // EMP damage and bionics
    internal const bool ENABLE_EMP_DAMAGE_TO_BIONICS_DEFAULT = true;
    internal const float EMP_DAMAGE_TO_BIONICS_CHANCE_DEFAULT = 0.45f;
    // hemorrhagic stroke
    internal const bool ENABLE_HEMORRHAGIC_STROKE_DEFAULT = true;
    internal const float HEMORRHAGIC_STROKE_CHANCE_DEFAULT = 0.25f;
    internal const float HEMORRHAGIC_STROKE_THRESHOLD_DEFAULT = 15f;
    // hydrostatic shock
    internal const bool ENABLE_HYDROSTATIC_SHOCK_DEFAULT = false;
    internal const float HYDROSTATIC_SHOCK_CHANCE_ON_DAMAGE_DEFAULT = 0.2f;
    // lung collapse
    internal const bool ENABLE_LUNG_COLLAPSE_DEFAULT = true;
    internal const float LUNG_COLLAPSE_CHANCE_ON_DAMAGE_DEFAULT = 0.4f;
    internal const float LUNG_COLLAPSE_MAX_SEVERITY_ROOT_DEFAULT = 0.85f;
    // spalling
    internal const bool ENABLE_SPALLING_DEFAULT = true;
    internal const float SPALLING_CHANCE_DEFAULT = 0.75f;
    internal const float ARMOR_HEALTH_SPALLING_THRESHOLD_DEFAULT = 0.95f;
    // tourniquet-related injuries and gangrene
    internal const bool TOURNIQUETS_CAN_CAUSE_GANGRENE_DEFAULT = true;
    internal const float DRY_GANGRENE_CHANCE_DEFAULT = 0.8f;
    internal const float DRY_GANGRENE_MEAN_TIME_TO_INFECTION_DEFAULT = 240000f;
    internal const float MEAN_TIME_BETWEEN_GANGRENE_ON_TOURNIQUET_DEFAULT = 10000f;
    // miscellaneous
    internal const bool ENABLE_FIRE_INHALATION_DEFAULT = true;
    internal const bool BIOTECH_ENABLE_INTEGRATION_DEFAULT = false;
    internal const bool ANOMALY_ENABLE_CONDITIONS_FOR_SHAMBLERS_DEFAULT = true;
    internal const float CLOSED_INTERNAL_WOULD_BLEEDING_MODIFIER_DEFAULT = 0.75f;
    internal const float INTESTINAL_SPILLING_CHANCE_ON_DAMAGE_DEFAULT = 0.45f;
    internal const bool ENABLE_PARALYSIS_DEFAULT = true;
    internal const float PARALYSIS_DAMAGE_TRESHOLD_50_PERCENT_DEFAULT = 6f;
}