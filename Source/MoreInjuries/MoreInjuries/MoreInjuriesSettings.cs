using Verse;

namespace MoreInjuries;

public class MoreInjuriesSettings : ModSettings
{
    public bool AdrenalineBool = false;
    public bool HydroStaticShockBool = false;
    public bool BruiseStroke = true;
    public bool choking = true;
    public bool somesound = false;
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
    public bool advancedShock = true;
    public float hypoxiaChance = 0.65f;
    public bool anyColoration = true;
    public bool fireInhalation = true;
    internal bool enableLogging;
    internal bool enableVerboseLogging;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref AdrenalineBool, "adrbool");
        Scribe_Values.Look(ref HydroStaticShockBool, "hydrobool");
        Scribe_Values.Look(ref BruiseStroke, "bruisebool");
        Scribe_Values.Look(ref choking, "chokebool");
        Scribe_Values.Look(ref somesound, "asounf");
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
        Scribe_Values.Look(ref hypoxiaChance, "hypoxia");
        Scribe_Values.Look(ref advancedShock, "advancxedshock");
        Scribe_Values.Look(ref smolBoniShits, "smallBonyShits");
        Scribe_Values.Look(ref fireInhalation, "firesnorty");

        base.ExposeData();
    }
}

