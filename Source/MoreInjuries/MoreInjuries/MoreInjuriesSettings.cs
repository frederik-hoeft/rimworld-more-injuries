using Verse;

namespace MoreInjuries;

public class MoreInjuriesSettings : ModSettings
{
    public bool AdrenalineBool = false;
    public bool HydroStaticShockBool = false;
    public bool BruiseStroke = true;
    public bool choking = true;
    public bool ChokingSoundsEnabled = false;
    public bool lungcollapse = true;
    public bool spall = false;
    public float MinSpallHealth;
    public bool HearDMG = false;
    public bool toggleFractures = true;
    public bool EMPdisablesBionics = true;
    public bool fuckYourFun = false;
    public bool smolBoniShits = false;
    public float PlugMult = 0.75f;
    public float fractureTreshold = 8f;
    public bool individualFloatMenus = false;
    public bool HypovolemicShockEnabled = true;
    public float OrganHypoxiaChance = 0.65f;
    public bool anyColoration = true;
    public bool enableFireInhalation = true;
    internal bool enableLogging;
    internal bool enableVerboseLogging;
    internal float OrganHypoxiaChanceReductionFactor;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref AdrenalineBool, "adrbool");
        Scribe_Values.Look(ref HydroStaticShockBool, "hydrobool");
        Scribe_Values.Look(ref BruiseStroke, "bruisebool");
        Scribe_Values.Look(ref choking, "chokebool");
        Scribe_Values.Look(ref ChokingSoundsEnabled, "asounf");
        Scribe_Values.Look(ref spall, "amungsuussssad");
        Scribe_Values.Look(ref lungcollapse, "amungsuussssad2");
        Scribe_Values.Look(ref MinSpallHealth, "stroekaonfweiabdwuabduwbao");
        Scribe_Values.Look(ref HearDMG, "fghjkdfrghjkfghjk");
        Scribe_Values.Look(ref EMPdisablesBionics, "amIdisabled");
        Scribe_Values.Look(ref toggleFractures, "FracturesBool");
        Scribe_Values.Look(ref fuckYourFun, "invisibleGunShotsBool");
        Scribe_Values.Look(ref PlugMult, "thisNamesoundsAwful");
        Scribe_Values.Look(ref fractureTreshold, "tresholdFracture");
        Scribe_Values.Look(ref individualFloatMenus, "floatmenu");
        Scribe_Values.Look(ref OrganHypoxiaChance, "hypoxia");
        Scribe_Values.Look(ref HypovolemicShockEnabled, "advancxedshock");
        Scribe_Values.Look(ref smolBoniShits, "smallBonyShits");
        Scribe_Values.Look(ref enableFireInhalation, "firesnorty");

        base.ExposeData();
    }
}

