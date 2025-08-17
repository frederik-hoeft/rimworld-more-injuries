using MoreInjuries.Roslyn.Metadata.KeyedMembers;
using MoreInjuries.Roslyn.Metadata.Settings;
using Verse;

namespace MoreInjuries;

[GeneratedSettings]
[KeyedMembers(Visibility = Visibility.Internal)]
public partial class MoreInjuriesSettings : ModSettings
{
    // logging
    [SettingsEntry<bool>(DefaultValue = false)] 
    internal partial ref bool EnableLogging { get; }

    [SettingsEntry<bool>(DefaultValue = false)] 
    internal partial ref bool EnableVerboseLogging { get; }

    // adrenaline and epinephrine
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableAdrenaline { get; }

    [SettingsEntry<float>(DefaultValue = 0.75f)]
    internal partial ref float AdrenalineChanceOnDamage { get; }

    [SettingsEntry<float>(DefaultValue = 15f)]
    internal partial ref float CertainAdrenalineThreshold { get; }
    
    // blood transfusions
    [SettingsEntry<int>(DefaultValue = 5)]
    internal partial ref int BloodTransfusionHarvestMinimumSkill { get; }
    
    // hydrostatic shock
    [SettingsEntry<bool>(DefaultValue = false)]
    internal partial ref bool EnableHydrostaticShock { get; }

    [SettingsEntry<float>(DefaultValue = 0.2f)]
    internal partial ref float HydrostaticShockChanceOnDamage { get; }
    
    // EMP
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableEmpDamageToBionics { get; }

    [SettingsEntry<float>(DefaultValue = 0.45f)]
    internal partial ref float EmpDamageToBionicsChance { get; }
    
    // hemorrhagic stroke
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableHemorrhagicStroke { get; }

    [SettingsEntry<float>(DefaultValue = 0.25f)]
    internal partial ref float HemorrhagicStrokeChance { get; }

    [SettingsEntry<float>(DefaultValue = 15f)]
    internal partial ref float HemorrhagicStrokeThreshold { get; }
    
    // concussions
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableConcussion { get; }

    [SettingsEntry<float>(DefaultValue = 0.75f)]
    internal partial ref float ConcussionChance { get; }

    [SettingsEntry<float>(DefaultValue = 6f)]
    internal partial ref float ConcussionThreshold { get; }
    
    // choking
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableChoking { get; }

    [SettingsEntry<float>(DefaultValue = 0.75f)]
    internal partial ref float ChokingChanceOnDamage { get; }

    [SettingsEntry<float>(DefaultValue = 0.2f)]
    internal partial ref float ChokingMinimumBleedRate { get; }

    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableChokingSounds { get; }

    [SettingsEntry<float>(DefaultValue = 0.25f)]
    internal partial ref float SuctionDeviceMinimumSuccessRate { get; }
    
    // lung collapse
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableLungCollapse { get; }

    [SettingsEntry<float>(DefaultValue = 0.4f, Name = "LungCollapseChanceOnDamage")]
    internal partial ref float LungCollapseChanceOnThermobaricDamage { get; }

    [SettingsEntry<float>(DefaultValue = 0.2f)]
    internal partial ref float LungCollapseChanceOnPerforatingDamage { get; }

    [SettingsEntry<float>(DefaultValue = 0.85f)]
    internal partial ref float LungCollapseMaxSeverityRoot { get; }
    
    // spalling
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableSpalling { get; }

    [SettingsEntry<float>(DefaultValue = 0.95f)]
    internal partial ref float ArmorHealthSpallingThreshold { get; }

    [SettingsEntry<float>(DefaultValue = 0.75f)]
    internal partial ref float SpallingChance { get; }
    
    // hearing damage
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableBasicHearingDamage { get; }

    [SettingsEntry<bool>(DefaultValue = false)]
    internal partial ref bool EnableAdvancedHearingDamage { get; }

    [SettingsEntry<float>(DefaultValue = 0.25f)]
    internal partial ref float HearingDamageTemporarySeverityFactor { get; }

    [SettingsEntry<float>(DefaultValue = 0.75f)]
    internal partial ref float HearingDamageTemporarySeverityFactorExplosions { get; }

    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool HearingDamageMayBecomePermanent { get; }

    [SettingsEntry<float>(DefaultValue = 0.25f)]
    internal partial ref float HearingDamagePermanentChanceFactor { get; }
    
    // fractures
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableFractures { get; }

    [SettingsEntry<float>(DefaultValue = 10f)]
    internal partial ref float FractureDamageTreshold { get; }

    [SettingsEntry<float>(DefaultValue = 0.25f)]
    internal partial ref float FractureChanceOnDamage { get; }

    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableBoneFragmentLacerations { get; }

    [SettingsEntry<float>(DefaultValue = 0.5f)]
    internal partial ref float SplinteringFractureChance { get; }

    [SettingsEntry<float>(DefaultValue = 0.5f)]
    internal partial ref float BoneFragmentLacerationChancePerBodyPart { get; }

    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableApplySplintJob { get; }
    
    // hypovolemic shock, cardiac arrest, and the trauma triad of death
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableHypovolemicShock { get; }

    [SettingsEntry<float>(DefaultValue = 0.35f)]
    internal partial ref float OrganHypoxiaChance { get; }

    [SettingsEntry<float>(DefaultValue = 0.5f)]
    internal partial ref float OrganHypoxiaChanceReductionFactor { get; }

    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableCardiacArrestOnHighBloodLoss { get; }

    [SettingsEntry<float>(DefaultValue = 0.05f)]
    internal partial ref float CardiacArrestChanceOnHighBloodLoss { get; }

    [SettingsEntry<float>(DefaultValue = 0.5f)]
    internal partial ref float DefibrillatorMinimumSuccessRate { get; }

    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableAdvancedTraumaSimulation { get; }

    [SettingsEntry<float>(DefaultValue = 0.015f)]
    internal partial ref float HypoxiaAcidosisConversionFactor { get; }
    
    // neural damage and permanent brain injuries
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableNeuralDamage { get; }

    [SettingsEntry<float>(DefaultValue = 0.5f)]
    internal partial ref float NeuralDamageChanceReductionFactor { get; }

    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool NeuralDamageEnablePersonalityShift { get; }

    // tourniquet-related injuries
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool TourniquetsCanCauseGangrene { get; }

    [SettingsEntry<float>(DefaultValue = 10000f)]
    internal partial ref float MeanTimeBetweenGangreneOnTourniquet { get; }

    [SettingsEntry<float>(DefaultValue = 0.8f)]
    internal partial ref float DryGangreneChance { get; }

    [SettingsEntry<float>(DefaultValue = 240000f)]
    internal partial ref float DryGangreneMeanTimeToInfection { get; }

    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableTourniquetGizmo { get; }

    [SettingsEntry<float>(DefaultValue = 0.5f)]
    internal partial ref float MinBleedRateForAutoTourniquet { get; }
    
    // miscellaneous
    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableFireInhalation { get; }

    [SettingsEntry<bool>(DefaultValue = false)]
    internal partial ref bool BiotechEnableIntegration { get; }

    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool AnomalyEnableConditionsForShamblers { get; }

    [SettingsEntry<bool>(DefaultValue = true)]
    internal partial ref bool EnableParalysis { get; }

    [SettingsEntry<float>(DefaultValue = 6f)]
    internal partial ref float ParalysisDamageTreshold50Percent { get; }

    [SettingsEntry<float>(DefaultValue = 0.45f)]
    internal partial ref float IntestinalSpillingChanceOnDamage { get; }

    [SettingsEntry<float>(DefaultValue = 0.75f)]
    internal partial ref float ClosedInternalWouldBleedingModifier { get; }

    public override partial void ExposeData();
}

