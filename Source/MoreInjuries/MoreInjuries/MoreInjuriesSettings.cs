using Verse;

namespace MoreInjuries;

public class MoreInjuriesSettings : ModSettings
{
    internal bool EnableAdrenaline = false;
    internal bool EnableHydrostaticShock = false;
    internal bool EnableEmpDamageToBionics = true;
    internal bool HideUndiagnosedInternalInjuries = false;
    internal float ClosedInternalWouldBleedingModifier = 0.75f;
    internal bool UseIndividualFloatMenus = false;

    internal bool EnableHemorrhagicStroke = true;
    internal float HemorrhagicStrokeChance = 0.07f;

    internal bool EnableChoking = true;
    internal float ChokingChanceOnDamage = 0.75f;
    internal bool EnableChokingSounds = false;

    internal bool EnableLungCollapse = true;

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
    internal float OrganHypoxiaChanceReductionFactor;
    internal bool EnableFireInhalation = true;
    internal bool EnableLogging;
    internal bool EnableVerboseLogging;

    public override void ExposeData()
    {
        // logging
        // TODO: settings
        Scribe_Values.Look(ref EnableLogging, nameof(EnableLogging));
        Scribe_Values.Look(ref EnableVerboseLogging, nameof(EnableVerboseLogging));
        // feature flags
        Scribe_Values.Look(ref EnableAdrenaline, nameof(EnableAdrenaline));
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

        Scribe_Values.Look(ref EnableLungCollapse, nameof(EnableLungCollapse));
        Scribe_Values.Look(ref EnableHydrostaticShock, nameof(EnableHydrostaticShock));
        Scribe_Values.Look(ref EnableHearingDamage, nameof(EnableHearingDamage));
        Scribe_Values.Look(ref EnableEmpDamageToBionics, nameof(EnableEmpDamageToBionics));
        Scribe_Values.Look(ref HideUndiagnosedInternalInjuries, nameof(HideUndiagnosedInternalInjuries));
        Scribe_Values.Look(ref ClosedInternalWouldBleedingModifier, nameof(ClosedInternalWouldBleedingModifier));
        Scribe_Values.Look(ref UseIndividualFloatMenus, nameof(UseIndividualFloatMenus));
        Scribe_Values.Look(ref OrganHypoxiaChance, nameof(OrganHypoxiaChance));
        Scribe_Values.Look(ref EnableHypovolemicShock, nameof(EnableHypovolemicShock));
        Scribe_Values.Look(ref EnableFireInhalation, nameof(EnableFireInhalation));
        // TODO: settings
        Scribe_Values.Look(ref OrganHypoxiaChanceReductionFactor, nameof(OrganHypoxiaChanceReductionFactor));

        base.ExposeData();
    }
}

