using System.Linq;
using RimWorld;
using HarmonyLib;
using Verse;
using UnityEngine;

namespace MoreInjuries;

public class MoreInjuriesMod : Mod
{
    public static MoreInjuriesSettings Settings { get; private set; } = null!;

    public MoreInjuriesMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<MoreInjuriesSettings>();

        Harmony harmony = new("Caulaflower.Extended_Injuries.oof");

        harmony.PatchAll();
    }
    public override void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listingStandard = new();
        listingStandard.Begin(inRect);
        listingStandard.Label("Green - mechanic is turned ON. Red the opposite");

        listingStandard.CheckboxLabeled("Toggle adrenaline mechanics", ref Settings.AdrenalineBool, "Toggle adrenaline mechanics");

        listingStandard.CheckboxLabeled("Toggle bone fractures", ref Settings.toggleFractures, "Toggle fractures");

        listingStandard.CheckboxLabeled("Toggle bone fragments from fractures", ref Settings.smolBoniShits, "Toggle bone fragments from fractures");

        listingStandard.CheckboxLabeled("Toggle bruise shock mechanics", ref Settings.BruiseStroke, "Toggle  bruise shock mechanics");

        listingStandard.CheckboxLabeled("Toggle EMP disabling bionics", ref Settings.EMPdisablesBionics, "Toggle EMP disabling bionics. Credits for the idea for the mechanic to I Play Minecraft");

        listingStandard.CheckboxLabeled("Toggle  choking on blood mechanics", ref Settings.choking, "Toggle  choking on blood mechanics");

        listingStandard.TextEntry("Base chance of armor creating spall " +
                "(at 1, the chance of creating spall is 0 with armor having 100% hp, 0.01 with armor 99% hp etc.) " + Settings.MinSpallHealth, 2);
        Settings.MinSpallHealth = listingStandard.Slider(Settings.MinSpallHealth, 0.1f, 1f);

        listingStandard.CheckboxLabeled("Toggle  choking sounds", ref Settings.ChokingSoundsEnabled, "Toggle  choking sounds");

        listingStandard.CheckboxLabeled("Toggle spalling mechanics", ref Settings.spall, "Toggle  spalling mechanics");

        listingStandard.CheckboxLabeled("Toggle lung collapses", ref Settings.lungcollapse, "Toggle lung collapses");

        listingStandard.CheckboxLabeled("Toggle hearing damage mechanics (requires game reload)", ref Settings.HearDMG, "Toggle hearing damage mechanics (requires game reload)");

        listingStandard.Label("Slider for 'plugging' internal injuries bleedrate mult: " + Settings.PlugMult.ToString(), -1,
            "Let's say a pawn gets shot in the stomach. Now he has a gunshot on stomach and on torso. If you put a bandage, hemostat or tend the torso hit stomach shot's value will be multiplied by this value");

        Settings.PlugMult = (float)Math.Round(listingStandard.Slider(Settings.PlugMult, 0f, 1f), 2);

        listingStandard.Label("Choose fracture damage treshold. Current treshold " + Settings.fractureTreshold);

        Settings.fractureTreshold = (float)Math.Ceiling(listingStandard.Slider(Settings.fractureTreshold, 1, 20));

        listingStandard.CheckboxLabeled("Show individual options for hemostat usage alongside 'Provide first aid option'", ref Settings.individualFloatMenus);

        listingStandard.CheckboxLabeled("Enable advanced shock mechanics (requires game reload)", ref Settings.HypovolemicShockEnabled);

        listingStandard.CheckboxLabeled("Toggle inhalation of fire's fuel when set on fire ", ref Settings.fireInhalation);

        listingStandard.Label("Chance of shock to cause organ hypoxia (every 300 ticks/5s) " + Settings.OrganHypoxiaChance);

        Settings.OrganHypoxiaChance = (float)Math.Round(listingStandard.Slider(Settings.OrganHypoxiaChance, 0f, 1f), 2);

        //listingStandard.CheckboxLabeled("Enable coloration of labels")

        if (Find.CurrentMap != null)
        {
            if (listingStandard.ButtonText("Fix misplaced bionics"))
            {
                System.Collections.Generic.IEnumerable<Pawn> A = Find.CurrentMap.mapPawns.AllPawns.Where(x => x.def == ThingDefOf.Human);
                foreach (Pawn human in A)
                {
                    System.Collections.Generic.List<Hediff> B = human.health.hediffSet.hediffs.FindAll(x => x.def.addedPartProps != null && x.def.HasModExtension<FixerModExt>());

                    foreach (Hediff hed in B)
                    {
                        FixerModExt supPartDef = hed.def.GetModExtension<FixerModExt>();

                        System.Collections.Generic.IEnumerable<BodyPartRecord> PosSupParts = human.health.hediffSet.GetNotMissingParts().Where(p => supPartDef.bodyparts.Contains(p.def));

                        if (PosSupParts.Count() <= 0)
                        {
                            human.health.RemoveHediff(hed);
                        }
                        else
                        {
                            hed.Part = PosSupParts.RandomElement();
                        }
                    }
                }
            }
        }
        listingStandard.End();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "More Injuries";
    }
}

