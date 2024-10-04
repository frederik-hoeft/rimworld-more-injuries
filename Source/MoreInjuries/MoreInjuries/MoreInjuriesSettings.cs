using Verse;

namespace MoreInjuries;

public class MoreInjuriesSettings : ModSettings
{
    internal bool EnableAdrenaline = false;
    internal float AdrenalineChanceOnDamage = 0.75f;

    internal bool EnableHydrostaticShock = false;
    internal float HydrostaticShockChanceOnDamage = 0.2f;

    internal bool HideUndiagnosedInternalInjuries = false;
    internal float ClosedInternalWouldBleedingModifier = 0.75f;
    internal bool UseIndividualFloatMenus = false;

    internal bool EnableEmpDamageToBionics = true;
    internal float EmpDamageToBionicsChance = 0.8f;

    internal bool EnableHemorrhagicStroke = true;
    internal float HemorrhagicStrokeChance = 0.07f;

    internal bool EnableChoking = true;
    internal float ChokingChanceOnDamage = 0.75f;
    internal bool EnableChokingSounds = false;

    internal bool EnableLungCollapse = true;
    internal float LungCollapseChanceOnDamage = 0.5f;
    internal float LungCollapseMaxSeverityRoot = 0.85f;

    internal bool EnableSpalling = false;
    internal float ArmorHealthSpallingThreshold = 0.95f;
    internal float SpallingChance = 0.75f;

    internal bool EnableHearingDamage = false;

    internal bool EnableFractures = true;
    internal float FractureDamageTreshold = 8f;
    internal float FractureChanceOnDamage = 0.75f;
    internal bool EnableBoneFragmentLacerations = false;
    internal float BoneFragmentLacerationChance = 0.1f;

    internal bool EnableHypovolemicShock = true;
    internal float OrganHypoxiaChance = 0.65f;
    internal float OrganHypoxiaChanceReductionFactor = 0.5f;
    internal bool EnableFireInhalation = true;
    internal bool EnableLogging;
    internal bool EnableVerboseLogging;

    internal float ParalysisDamageTreshold50Percent = 6f;

    internal float IntestinalSpillingChanceOnDamage = 0.45f;

    public override void ExposeData()
    {
        // logging
        Scribe_Values.Look(ref EnableLogging, nameof(EnableLogging));
        Scribe_Values.Look(ref EnableVerboseLogging, nameof(EnableVerboseLogging));
        // feature flags
        Scribe_Values.Look(ref EnableAdrenaline, nameof(EnableAdrenaline));
        Scribe_Values.Look(ref AdrenalineChanceOnDamage, nameof(AdrenalineChanceOnDamage));
        // fractures
        Scribe_Values.Look(ref EnableFractures, nameof(EnableFractures));
        Scribe_Values.Look(ref FractureDamageTreshold, nameof(FractureDamageTreshold));
        Scribe_Values.Look(ref FractureChanceOnDamage, nameof(FractureChanceOnDamage));
        Scribe_Values.Look(ref EnableBoneFragmentLacerations, nameof(EnableBoneFragmentLacerations));
        Scribe_Values.Look(ref BoneFragmentLacerationChance, nameof(BoneFragmentLacerationChance));
        // hemorrhagic stroke after blunt trauma
        Scribe_Values.Look(ref EnableHemorrhagicStroke, nameof(EnableHemorrhagicStroke));
        Scribe_Values.Look(ref HemorrhagicStrokeChance, nameof(HemorrhagicStrokeChance));
        // choking on blood
        Scribe_Values.Look(ref EnableChoking, nameof(EnableChoking));
        Scribe_Values.Look(ref EnableChokingSounds, nameof(EnableChokingSounds));
        Scribe_Values.Look(ref ChokingChanceOnDamage, nameof(ChokingChanceOnDamage));
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

        Scribe_Values.Look(ref EnableHearingDamage, nameof(EnableHearingDamage));
        Scribe_Values.Look(ref HideUndiagnosedInternalInjuries, nameof(HideUndiagnosedInternalInjuries));
        Scribe_Values.Look(ref ClosedInternalWouldBleedingModifier, nameof(ClosedInternalWouldBleedingModifier));
        Scribe_Values.Look(ref UseIndividualFloatMenus, nameof(UseIndividualFloatMenus));
        Scribe_Values.Look(ref OrganHypoxiaChance, nameof(OrganHypoxiaChance));
        Scribe_Values.Look(ref EnableHypovolemicShock, nameof(EnableHypovolemicShock));
        Scribe_Values.Look(ref EnableFireInhalation, nameof(EnableFireInhalation));
        Scribe_Values.Look(ref OrganHypoxiaChanceReductionFactor, nameof(OrganHypoxiaChanceReductionFactor));
        Scribe_Values.Look(ref ParalysisDamageTreshold50Percent, nameof(ParalysisDamageTreshold50Percent));
        Scribe_Values.Look(ref IntestinalSpillingChanceOnDamage, nameof(IntestinalSpillingChanceOnDamage));

        base.ExposeData();
    }
}

