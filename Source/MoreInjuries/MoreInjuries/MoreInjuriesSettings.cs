﻿using Verse;

namespace MoreInjuries;

public class MoreInjuriesSettings : ModSettings
{
    internal bool EnableAdrenaline = true;
    internal float AdrenalineChanceOnDamage = 0.75f;
    internal float CertainAdrenalineThreshold = 15f;

    internal bool EnableHydrostaticShock = false;
    internal float HydrostaticShockChanceOnDamage = 0.2f;

    internal bool HideUndiagnosedInternalInjuries = false;
    internal float ClosedInternalWouldBleedingModifier = 0.75f;
    internal bool UseIndividualFloatMenus = false;

    internal bool EnableEmpDamageToBionics = true;
    internal float EmpDamageToBionicsChance = 0.45f;

    internal bool EnableHemorrhagicStroke = true;
    internal float HemorrhagicStrokeChance = 0.25f;
    internal float HemorrhagicStrokeThreshold = 15f;

    internal bool EnableConcussion = true;
    internal float ConcussionChance = 0.75f;
    internal float ConcussionThreshold = 6f;

    internal bool EnableChoking = true;
    internal float ChokingChanceOnDamage = 0.75f;
    internal float ChokingMinimumBleedRate = 0.2f;
    internal bool EnableChokingSounds = true;
    internal float SuctionDeviceMinimumSuccessRate = 0.25f;

    internal bool EnableLungCollapse = true;
    internal float LungCollapseChanceOnDamage = 0.4f;
    internal float LungCollapseMaxSeverityRoot = 0.85f;

    internal bool EnableSpalling = true;
    internal float ArmorHealthSpallingThreshold = 0.95f;
    internal float SpallingChance = 0.75f;

    internal bool EnableBasicHearingDamage = true;
    internal bool EnableAdvancedHearingDamage = false;

    internal bool EnableFractures = true;
    internal float FractureDamageTreshold = 10f;
    internal float FractureChanceOnDamage = 0.25f;
    internal bool EnableBoneFragmentLacerations = true;
    internal float SplinteringFractureChance = 0.5f;
    internal float BoneFragmentLacerationChancePerBodyPart = 0.5f;

    internal bool EnableHypovolemicShock = true;
    internal float OrganHypoxiaChance = 0.35f;
    internal float OrganHypoxiaChanceReductionFactor = 0.5f;
    internal bool EnableCardiacArrestOnHighBloodLoss = true;
    internal float CardiacArrestChanceOnHighBloodLoss = 0.05f;
    internal float DefibrillatorMinimumSuccessRate = 0.5f;

    internal bool EnableFireInhalation = true;
    internal bool EnableLogging = false;
    internal bool EnableVerboseLogging = false;

    internal float ParalysisDamageTreshold50Percent = 6f;

    internal float IntestinalSpillingChanceOnDamage = 0.45f;

    public override void ExposeData()
    {
        // logging
        Scribe_Values.Look(ref EnableLogging, nameof(EnableLogging));
        Scribe_Values.Look(ref EnableVerboseLogging, nameof(EnableVerboseLogging));
        // adrenaline
        Scribe_Values.Look(ref EnableAdrenaline, nameof(EnableAdrenaline));
        Scribe_Values.Look(ref AdrenalineChanceOnDamage, nameof(AdrenalineChanceOnDamage));
        Scribe_Values.Look(ref CertainAdrenalineThreshold, nameof(CertainAdrenalineThreshold));
        // fractures
        Scribe_Values.Look(ref EnableFractures, nameof(EnableFractures));
        Scribe_Values.Look(ref FractureDamageTreshold, nameof(FractureDamageTreshold));
        Scribe_Values.Look(ref FractureChanceOnDamage, nameof(FractureChanceOnDamage));
        Scribe_Values.Look(ref EnableBoneFragmentLacerations, nameof(EnableBoneFragmentLacerations));
        Scribe_Values.Look(ref BoneFragmentLacerationChancePerBodyPart, nameof(BoneFragmentLacerationChancePerBodyPart));
        // hemorrhagic stroke after blunt trauma
        Scribe_Values.Look(ref EnableHemorrhagicStroke, nameof(EnableHemorrhagicStroke));
        Scribe_Values.Look(ref HemorrhagicStrokeChance, nameof(HemorrhagicStrokeChance));
        Scribe_Values.Look(ref HemorrhagicStrokeThreshold, nameof(HemorrhagicStrokeThreshold));
        // concussions after blunt trauma
        Scribe_Values.Look(ref EnableConcussion, nameof(EnableConcussion));
        Scribe_Values.Look(ref ConcussionChance, nameof(ConcussionChance));
        Scribe_Values.Look(ref ConcussionThreshold, nameof(ConcussionThreshold));
        // choking on blood
        Scribe_Values.Look(ref EnableChoking, nameof(EnableChoking));
        Scribe_Values.Look(ref EnableChokingSounds, nameof(EnableChokingSounds));
        Scribe_Values.Look(ref ChokingMinimumBleedRate, nameof(ChokingMinimumBleedRate));
        Scribe_Values.Look(ref ChokingChanceOnDamage, nameof(ChokingChanceOnDamage));
        Scribe_Values.Look(ref SuctionDeviceMinimumSuccessRate, nameof(SuctionDeviceMinimumSuccessRate));
        // spalling
        Scribe_Values.Look(ref EnableSpalling, nameof(EnableSpalling));
        Scribe_Values.Look(ref ArmorHealthSpallingThreshold, nameof(ArmorHealthSpallingThreshold));
        Scribe_Values.Look(ref SpallingChance, nameof(SpallingChance));
        // hydrostatic shock
        Scribe_Values.Look(ref EnableHydrostaticShock, nameof(EnableHydrostaticShock));
        Scribe_Values.Look(ref HydrostaticShockChanceOnDamage, nameof(HydrostaticShockChanceOnDamage));
        // lung collapse
        Scribe_Values.Look(ref EnableLungCollapse, nameof(EnableLungCollapse));
        Scribe_Values.Look(ref LungCollapseChanceOnDamage, nameof(LungCollapseChanceOnDamage));
        Scribe_Values.Look(ref LungCollapseMaxSeverityRoot, nameof(LungCollapseMaxSeverityRoot));
        // EMP
        Scribe_Values.Look(ref EnableEmpDamageToBionics, nameof(EnableEmpDamageToBionics));
        Scribe_Values.Look(ref EmpDamageToBionicsChance, nameof(EmpDamageToBionicsChance));
        // hearing damage
        Scribe_Values.Look(ref EnableBasicHearingDamage, nameof(EnableBasicHearingDamage));
        Scribe_Values.Look(ref EnableAdvancedHearingDamage, nameof(EnableAdvancedHearingDamage));
        // hypovolemic shock
        Scribe_Values.Look(ref EnableHypovolemicShock, nameof(EnableHypovolemicShock));
        Scribe_Values.Look(ref OrganHypoxiaChance, nameof(OrganHypoxiaChance));
        Scribe_Values.Look(ref OrganHypoxiaChanceReductionFactor, nameof(OrganHypoxiaChanceReductionFactor));
        Scribe_Values.Look(ref EnableCardiacArrestOnHighBloodLoss, nameof(EnableCardiacArrestOnHighBloodLoss));
        Scribe_Values.Look(ref CardiacArrestChanceOnHighBloodLoss, nameof(CardiacArrestChanceOnHighBloodLoss));
        Scribe_Values.Look(ref DefibrillatorMinimumSuccessRate, nameof(DefibrillatorMinimumSuccessRate));

        Scribe_Values.Look(ref HideUndiagnosedInternalInjuries, nameof(HideUndiagnosedInternalInjuries));
        Scribe_Values.Look(ref ClosedInternalWouldBleedingModifier, nameof(ClosedInternalWouldBleedingModifier));
        Scribe_Values.Look(ref UseIndividualFloatMenus, nameof(UseIndividualFloatMenus));
        Scribe_Values.Look(ref EnableFireInhalation, nameof(EnableFireInhalation));
        Scribe_Values.Look(ref ParalysisDamageTreshold50Percent, nameof(ParalysisDamageTreshold50Percent));
        Scribe_Values.Look(ref IntestinalSpillingChanceOnDamage, nameof(IntestinalSpillingChanceOnDamage));

        base.ExposeData();
    }
}

