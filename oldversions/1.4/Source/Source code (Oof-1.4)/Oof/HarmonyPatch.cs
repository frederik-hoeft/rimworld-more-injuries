using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse.AI;
using Verse.Sound;
using HarmonyLib;
using HarmonyMod;
using Verse;
using UnityEngine;
namespace Oof
{
    #region mod class
    public class OofMod : Mod
    {
        public static ToggleSettings settings;
        public OofMod(ModContentPack content) : base(content)
        {
           

            settings = GetSettings<ToggleSettings>();

            var harmony = new Harmony("Caulaflower.Extended_Injuries.oof");

            harmony.PatchAll();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("Green - mechanic is turned ON. Red the opposite");

            listingStandard.CheckboxLabeled("Toggle adrenaline mechanics", ref settings.AdrenalineBool, "Toggle adrenaline mechanics");

            listingStandard.CheckboxLabeled("Toggle bone fractures", ref settings.toggleFractures, "Toggle fractures");

            listingStandard.CheckboxLabeled("Toggle bone fragments from fractures", ref settings.smolBoniShits, "Toggle bone fragments from fractures");

            listingStandard.CheckboxLabeled("Toggle bruise shock mechanics", ref settings.BruiseStroke, "Toggle  bruise shock mechanics");

            listingStandard.CheckboxLabeled("Toggle EMP disabling bionics", ref settings.EMPdisablesBionics, "Toggle EMP disabling bionics. Credits for the idea for the mechanic to I Play Minecraft");

            listingStandard.CheckboxLabeled("Toggle  choking on blood mechanics", ref settings.choking, "Toggle  choking on blood mechanics");

            listingStandard.TextEntry("Base chance of armor creating spall " +
                    "(at 1, the chance of creating spall is 0 with armor having 100% hp, 0.01 with armor 99% hp etc.) " + settings.MinSpallHealth, 2);
                settings.MinSpallHealth = listingStandard.Slider(settings.MinSpallHealth, 0.1f, 1f);
           
            listingStandard.CheckboxLabeled("Toggle  choking sounds", ref settings.somesound, "Toggle  choking sounds");

            listingStandard.CheckboxLabeled("Toggle spalling mechanics", ref settings.spall, "Toggle  spalling mechanics");

            listingStandard.CheckboxLabeled("Toggle lung collapses", ref settings.lungcollapse, "Toggle lung collapses");

            listingStandard.CheckboxLabeled("Toggle hearing damage mechanics (requires game reload)", ref settings.HearDMG, "Toggle hearing damage mechanics (requires game reload)");

            listingStandard.Label("Slider for 'plugging' internal injuries bleedrate mult: " + settings.PlugMult.ToString(), -1, 
                "Let's say a pawn gets shot in the stomach. Now he has a gunshot on stomach and on torso. If you put a bandage, hemostat or tend the torso hit stomach shot's value will be multiplied by this value");

            settings.PlugMult =  (float)Math.Round( listingStandard.Slider(settings.PlugMult, 0f, 1f), 2);

            listingStandard.Label("Choose fracture damage treshold. Current treshold " + settings.fractureTreshold);

            settings.fractureTreshold = (float)Math.Ceiling(listingStandard.Slider(settings.fractureTreshold, 1, 20));

            listingStandard.CheckboxLabeled("Show individual options for hemostat usage alongside 'Provide first aid option'", ref settings.individualFloatMenus);

            listingStandard.CheckboxLabeled("Enable advanced shock mechanics (requires game reload)", ref settings.advancedShock);

            listingStandard.CheckboxLabeled("Toggle inhalation of fire's fuel when set on fire ", ref settings.fireInhalation);

            listingStandard.Label("Chance of shock to cause organ hypoxia (every 300 ticks/5s) " + settings.hypoxiaChance);

            settings.hypoxiaChance = (float)Math.Round(listingStandard.Slider(settings.hypoxiaChance, 0f, 1f), 2);

            //listingStandard.CheckboxLabeled("Enable coloration of labels")

            if (Find.CurrentMap != null)
            {
                if(listingStandard.ButtonText("Fix misplaced bionics"))
                {
                    var A = Find.CurrentMap.mapPawns.AllPawns.Where(x => x.def == ThingDefOf.Human);
                    foreach (Pawn human in A)
                    {
                        var B = human.health.hediffSet.hediffs.FindAll(x => x.def.addedPartProps != null && x.def.HasModExtension<FixerModExt>());

                        foreach (Hediff hed in B)
                        {
                            var supPartDef = hed.def.GetModExtension<FixerModExt>();

                            var PosSupParts = human.health.hediffSet.GetNotMissingParts().Where(p => supPartDef.bodyparts.Contains(p.def));


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
    public class ToggleSettings : ModSettings
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

    #endregion

    #region patch damage
    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public static class Patch_Thing_TakeDamage
    {
        public static bool Active = false;

        static void Postfix(Thing __instance, DamageWorker.DamageResult __result)
        {
            if (!Active)
                return;

            if (__instance is Pawn compHolder)
            {
                var comp = compHolder.GetComp<InjuriesComp>();
                comp.PostDamageFull(__result);
            }

            Active = false;
        }
    }

    #endregion

    #region patch menu
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class FloatMenuMakerCarryAdder
    {
        // Token: 0x06000009 RID: 9 RVA: 0x00002150 File Offset: 0x00000350
        [HarmonyPostfix]
        public static void AddHumanlikeOrdersPostfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            bool flag = !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || pawn.WorkTagIsDisabled(WorkTags.Caring) || pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) || !pawn.workSettings.WorkIsActive(WorkTypeDefOf.Doctor);
            if (!flag)
            {
                foreach (LocalTargetInfo localTargetInfo in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true, null))
                {
                    Pawn target = (Pawn)localTargetInfo.Thing;
                    bool flag2 = !target.health.hediffSet.HasHediff(Caula_DefOf.ChokingOnBlood);
                    if(pawn.inventory.innerContainer.ToList().FindAll(AAA => AAA.def == Caula_DefOf.suctiondevice) != null)
                    {
                        bool flag69nice = pawn.inventory.innerContainer.ToList().FindAll(AAA => AAA.def == Caula_DefOf.suctiondevice).Count > 0;
                        if (flag69nice)
                        {
                            if (!flag2)
                            {
                                bool flag3 = !pawn.CanReserveAndReach(target, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true);
                                if (!flag3)
                                {
                                    JobDef stabilizeJD = Caula_DefOf.ClearAirway;
                                    Action action = delegate ()
                                    {
                                        Job job = new Job(stabilizeJD, target);
                                        job.count = 1;
                                        pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                                    };
                                    string label = "Clear airways";
                                    opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, MenuOptionPriority.RescueOrCapture, null, target, 0f, null, null), pawn, target, "ReservedBy"));
                                }
                            }
                        }
                        
                    }
                    
                   
                }
            }
        }
    }

    #endregion
    public class InjuriesComp : ThingComp
    {
        #region fields and misc stuff
        public InjuriesCompProps Props => (InjuriesCompProps)this.props;

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            
            Pawn papa = this.parent as Pawn;
            if (selPawn != papa)
            {
                if (papa.health.hediffSet.hediffs.Any(o => o.def.label == "Heart attack" | o.def == Caula_DefOf.ChokingOnBlood))
                {
                    yield return new FloatMenuOption("Perform CPR", delegate
                    {
                       
                        selPawn.jobs.StartJob(new Job(def: DefDatabase<JobDef>.AllDefs.ToList().Find(K => K.defName == "DOCPR"), targetA: papa), JobCondition.InterruptForced);
                    });
                }
            }
            if(selPawn.skills.GetSkill(SkillDefOf.Medicine).Level > 0)
            {
                if (papa.health.hediffSet.hediffs.Any(o => o.def.label == "Bone fracture") && selPawn.inventory.innerContainer.Any(tt33 => tt33.def.defName == "splint"))
                {
                    yield return new FloatMenuOption("Fix fracture", delegate
                    {
                        selPawn.jobs.StartJob(new Job(def: DefDatabase<JobDef>.AllDefs.ToList().Find(K => K.defName == "cbvnm"), targetA: papa), JobCondition.InterruptForced);
                    });
                }
            }
           
        }
        
      

        public DamageDef damDef;

        public float touse;

        public float bullet_mult = 0.05f;

        public int concussions_suffered = 0;

        
        public Need pawns_need;
        public override void Initialize(CompProperties props)
        {
            
            Pawn lowan = this.parent as Pawn;
            //pawns_need.CurLevelPercentage = 0.5f;
           
            //lowan.needs.TryGetNeed<Need_Adrenaline>.
            base.Initialize(props);
        }

        public void DumpAdrenaline(float DealtDamageChance)
        {
            Pawn lowan = this.parent as Pawn;
           
            if (Rand.Chance(DealtDamageChance))
            {
                if (!lowan.health.hediffSet.HasHediff(Caula_DefOf.adrenalinedump))
                {
                    lowan.health.AddHediff(HediffMaker.MakeHediff(Caula_DefOf.adrenalinedump, lowan));
                    Hediff AdrenalineOnPawn = lowan.health.hediffSet.GetFirstHediffOfDef(Caula_DefOf.adrenalinedump);
                    float bloat = Rand.Range(DealtDamageChance * - 10f, DealtDamageChance * 2);
                    AdrenalineOnPawn.Severity = bloat;
                   
                    

                }
                else
                {
                    float bloat = Rand.Range(DealtDamageChance * -10f, DealtDamageChance * 2);
                    lowan.health.hediffSet.GetFirstHediffOfDef(Caula_DefOf.adrenalinedump).Severity += bloat;
                }
            }
            
        }
        public void Bruise()
        {
            Pawn targetPawn = this.parent as Pawn;
            
            List<Hediff> Bruises = targetPawn.health.hediffSet.hediffs.FindAll(Bruise => Bruise.def == HediffDefOf.Bruise);
            List<Hediff> SevereBruises = Bruises.FindAll(SevereBruise => SevereBruise.Severity >= 14);
            List<Hediff> LegBruises = Bruises.FindAll(LegBruise => LegBruise.sourceBodyPartGroup == BodyPartGroupDefOf.Legs);
            if (Bruises?.Count > 15 | SevereBruises?.Count > 10 | LegBruises.Count > 5)
            {
                if (Rand.Chance(0.07f))
                {
                    targetPawn.health.AddHediff(Caula_DefOf.hemorrhagicstroke, targetPawn.health.hediffSet.GetBrain());
                }
            }
           

        }
        public void PuttingOnTourniqet()
        {
            Pawn targetPawn = this.parent as Pawn;
            List<Hediff_Injury> TendableInjuries = new List<Hediff_Injury>();
            if (targetPawn.health.hediffSet.GetInjuriesTendable() != null)
            {
                TendableInjuries = targetPawn.health.hediffSet.GetInjuriesTendable() as List<Hediff_Injury>;
            }
           
           
           
            
            List<Hediff_Injury> Injuries_to_Turnip = new List<Hediff_Injury>();
           
            

        }
        public DamageInfo pope;
        

        public ToggleSettings Settings
        {
            get
            {
                return LoadedModManager.GetMod<OofMod>().GetSettings<ToggleSettings>();
            }
        }

        public override void PostExposeData()
        {

            Scribe_Values.Look<int>(ref concussions_suffered, "pawnsufferedthismanyconcussionsusedforTBImechanic");
            base.PostExposeData();
        }

        
        public void nreTEst(List<BodyPartRecord> dd)
        {
            Pawn targetPawn = (Pawn)this.parent;
            foreach (BodyPartRecord bodp in dd)
            {
                if (targetPawn.health.hediffSet.hediffs.FindAll(o => o.Part != null && o.Part == bodp).Any(K => K.Bleeding))
                {
                    Hediff brun = HediffMaker.MakeHediff(DefDatabase<HediffDef>.AllDefs.ToList().Find(gg => gg != null && gg.defName == "StomachAcidBurn"), targetPawn, bodp);
                    if (Rand.Chance(0.45f))
                    {
                        brun.Severity = Rand.Range(1, 7f);
                        targetPawn.health.AddHediff(brun);
                    }
                }


            }
        }
        public List<BodyPartRecord> nrechaseagain()
        {
            Pawn targetPawn = (Pawn)this.parent;
            return targetPawn.health.hediffSet.GetNotMissingParts().ToList().FindAll(x => (x.def?.defName ?? "null") == "smolinstestine" | (x.def?.defName ?? "null") == "largeinstestine"
                | (x.def?.defName ?? "null") == "Stomach"
                | (x.def?.defName ?? "null") == "Kidney"
                | (x.def?.defName ?? "null") == "Liver"
            );
        }
        public void InstestinalSpill(DamageWorker.DamageResult damage)
        {
            Pawn targetPawn = (Pawn)this.parent;
            if (damage.parts?.Any(x => x.def?.defName == "smolinstestine" | x.def?.defName == "largeinstestine" | x.def?.defName == "Stomach") ?? false)
            {

                List<BodyPartRecord> dd = nrechaseagain();
                //dd.Add(targetPawn.health.hediffSet.getpart().ToList().FindAll(x => x.def.defName == "smolinstestine" | x.def.defName == "largeinstestine" | x.def.defName == "Stomach"))
                nreTEst(dd);
               

            }
        }

        #endregion

        #region misc damage stuff which doesnt need damageresult
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            Pawn papa = this.parent as Pawn;

            #region some choking stuff
            if (Settings.choking)
            {
                foreach (Hediff_Injury injury in papa.health.hediffSet.GetInjuriesTendable())
                {



                    if (injury.Part.def.tags.Contains(BodyPartTagDefOf.BreathingSource) | injury.Part.def.tags.Contains(BodyPartTagDefOf.BreathingPathway) && injury.Bleeding && injury.BleedRate >= 0.20f)
                    {
                        if (Rand.Chance(0.70f))
                        {
                            papa.health.AddHediff(Caula_DefOf.ChokingOnBlood);
                            papa.health.hediffSet.GetFirstHediffOfDef(Caula_DefOf.ChokingOnBlood).TryGetComp<chokingcomp>().source = injury;
                            ////////log.Message(injury.ToString());
                        }

                    }


                }
            }
            #endregion

            #region EMP disabling all bionics
            if (OofMod.settings.EMPdisablesBionics)
            {
                if (dinfo.Def == DamageDefOf.EMP | dinfo.Def.defName == "Electrical")
                {
                    foreach (Hediff part in papa.health.hediffSet.hediffs.FindAll(x => x.def.addedPartProps != null && x.def.addedPartProps.betterThanNatural && x.Part != null))
                    {
                        var hediff = HediffMaker.MakeHediff(Caula_DefOf.EMPTurnOff, papa, part.Part);
                        hediff.Severity = 1f;
                        papa.health.AddHediff(hediff, part.Part);

                    }
                }
            }
            #endregion
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
        }

        public void FractureMeth(DamageWorker.DamageResult damage)
        {
            if (Settings.toggleFractures)
            {
                if (damage.totalDamageDealt < OofMod.settings.fractureTreshold)
                {
                    return;
                }
                if (((damDef?.armorCategory?.armorRatingStat ?? null) == StatDefOf.ArmorRating_Sharp) | (damDef?.armorCategory?.armorRatingStat ?? null) == StatDefOf.ArmorRating_Blunt)
                {
                    Pawn targetPawn = (Pawn)this.parent;
                    if (damage?.parts?.Any(x => x.def.IsSolid(x, targetPawn.health.hediffSet.hediffs) && !x.def.IsSkinCovered(x, targetPawn.health.hediffSet) && x.def.bleedRate == 0) ?? false)
                    {
                        var partsbony = damage.parts.FindAll(x => x.def.IsSolid(x, targetPawn.health.hediffSet.hediffs) && !x.def.IsSkinCovered(x, targetPawn.health.hediffSet) && x.def.bleedRate == 0);

                        foreach (BodyPartRecord bone in partsbony)
                        {
                            Hediff fracture = HediffMaker.MakeHediff(DefDatabase<HediffDef>.AllDefs.ToList().Find(x => x.defName == "Fracture"), targetPawn, bone);
                            targetPawn.health.AddHediff(fracture);
                            somedefof.MoreInjuries_BoneSnap.PlayOneShot(new TargetInfo(targetPawn.PositionHeld, targetPawn.Map));
                            if (OofMod.settings.smolBoniShits)
                            {
                                foreach (BodyPartRecord part in bone.parent.GetDirectChildParts())
                                {
                                    if (Rand.Chance(0.10f))
                                    {
                                        Hediff shards = HediffMaker.MakeHediff(DefDatabase<HediffDef>.AllDefs.ToList().Find(x => x.defName == "BoneCut"), targetPawn, part);
                                        shards.Severity = Rand.Range(1f, 5f);
                                        targetPawn.health.AddHediff(shards);

                                    }
                                    foreach (BodyPartRecord part2 in part.GetDirectChildParts())
                                    {
                                        if (Rand.Chance(0.10f))
                                        {
                                            Hediff shards = HediffMaker.MakeHediff(DefDatabase<HediffDef>.AllDefs.ToList().Find(x => x.defName == "BoneCut"), targetPawn, part2);
                                            shards.Severity = Rand.Range(1f, 5f);
                                            targetPawn.health.AddHediff(shards);

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public Pawn dad => this.parent as Pawn;

        public void ExtendedExplosion(DamageInfo info)
        {
            if (
                (info.Def.defName == "Bomb"
                |
                info.Def.defName == "Thermobaric")
                &&
                    false
                )
            {
                var Lungs = dad.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName == "Lung");

                if (!Lungs.EnumerableNullOrEmpty())
                {
                    foreach (var lung in Lungs)
                    {
                        var hediff = HediffMaker.MakeHediff(Caula_DefOf.Crush, dad, lung);

                        hediff.Severity = Rand.Range(1f, (lung.def.hitPoints * 0.75f));

                        dad.health.AddHediff(hediff, lung);
                    }
                }

                
            }
        }

        public void burnLungs(DamageInfo burninfo)
        {
            if ((burninfo.Def?.hediff ?? null) == HediffDefOf.Burn)
            {
                var lungs = this.dad.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName == "Lung");
                //Log.Message("1");
                foreach (var lung in lungs)
                {
                    var lungBurnHediff = new Hediff { def = HediffDefOf.Burn, Severity = 200f, Part = lung, pawn = dad };

                    if (dad.health.hediffSet.hediffs.Any(x => x.Part?.def?.defName == "Lung" && x.def == HediffDefOf.Burn))
                    {
                        //Log.Message("2");
                        var lungburns = dad.health.hediffSet.hediffs.FindAll(x => x.Part?.def?.defName == "Lung" && x.def == HediffDefOf.Burn);

                        foreach (var idk in lungburns)
                        {
                            idk.Severity += 8f;
                        }
                        //Log.Message("3");
                    }
                    else
                    {
                        //Log.Message("2");
                        dad.health.AddHediff(lungBurnHediff, lung);
                    }
                }
            }
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            damDef = dinfo.Def;
            Oof.Patch_Thing_TakeDamage.Active = true;
            this.pope = dinfo;
            Pawn papa = this.parent as Pawn;

            ExtendedExplosion(dinfo);

            touse = dinfo.ArmorPenetrationInt;
            #region spall
            Pawn owan = (Pawn)this.parent;
            //log.Message("test 1");
            if ((owan.apparel?.WornApparel.Count ?? 0) > 0)
            {
                //log.Message("test 2");
                List<Apparel> armored = owan.apparel.WornApparel?.FindAll(k => k.def.apparel.CoversBodyPart(owan.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined).ToList().Find(P => P.def == BodyPartDefOf.Torso)));
                float touse2 = 0f;
                //log.Message("test 3");
                Apparel aparpub = new Apparel();
                if ((armored?.Count ?? 0) > 0)
                {
                    foreach (Apparel apar in armored)
                    {
                        if (apar.GetStatValue(StatDefOf.ArmorRating_Sharp) > touse2)
                        {
                            touse2 = apar.GetStatValue(StatDefOf.ArmorRating_Sharp);
                            aparpub = apar;
                        }
                    }
                }
                //log.Message("test 4");



                //log.Message("test 5");
                if (Settings.spall)
                {
                    if (dinfo.Def != null && aparpub != null)
                    {
                        #region disable spall for cataphracts and similar

                        //log.Message("test 5A1");

                        bool dospall = ((aparpub?.def?.techLevel ?? TechLevel.Animal) > TechLevel.Industrial);
                        //log.Message("test 5A1b complete");
                        #endregion

                        #region attempt to make pistols not apply spall with CE
                        bool CE_PistolBool = true;
                        //log.Message("test 5A2");
                        if (ModLister.HasActiveModWithName("Combat Extended"))
                        {
                            if (dinfo.Amount < 13)
                            {
                                CE_PistolBool = false;
                            }
                        }
                        //log.Message("test 5A3");
                        #endregion seems to function alright

                        if ((dinfo.Def == DamageDefOf.Bullet && touse < touse2) && CE_PistolBool && dospall)
                        {

                            //log.Message("test 5A4");
                            if (Rand.Chance((Settings.MinSpallHealth - ((aparpub.HitPoints * 1f) / (aparpub.def.BaseMaxHitPoints * 1f)))))
                            {
                                ///////log.Message("Source".Colorize(Color.yellow) + dinfo.Weapon ?? "penis.");

                                #region Angle calculations

                                var BulletAngleNinety = dinfo.Angle;

                                if (BulletAngleNinety < 90f)
                                {
                                    ////log.Message("hello " + BulletAngleNinety);
                                }
                                if (BulletAngleNinety > 91f && BulletAngleNinety < 180f)
                                {
                                    ////log.Message("hello " + BulletAngleNinety);
                                    BulletAngleNinety -= 90f;
                                }
                                if (BulletAngleNinety > 181f && BulletAngleNinety < 270f)
                                {
                                    ////log.Message("hello " + BulletAngleNinety);
                                    BulletAngleNinety -= 180f;
                                }
                                if (BulletAngleNinety > 271f && BulletAngleNinety < 359f)
                                {
                                    ////log.Message("hello " + BulletAngleNinety);
                                    BulletAngleNinety -= 270f;

                                }



                                BulletAngleNinety /= 90f;

                                BulletAngleNinety = (float)Math.Round(BulletAngleNinety, 1);

                                ////log.Message(BulletAngleNinety.ToString().Colorize(Color.magenta));

                                bullet_mult = BulletAngleNinety;

                                #endregion
                                #region Blunt pen and damage multipliers


                                ///Blunt pen isn't vanilla. WTF.
                                if (ModLister.HasActiveModWithName("Combat Extended"))
                                {
                                    ///in the patch, put this as an "out"
                                    float BluntPenRatio = 1f;

                                    bullet_mult *= BluntPenRatio;
                                }

                                bullet_mult *= (dinfo.Amount / 18);

                                var armorstopvalue = (aparpub.HitPoints * 1f) / (aparpub.def.BaseMaxHitPoints * 1f);

                                ////log.Message("Armor HP percentage: " + armorstopvalue.ToString().Colorize(Color.green));

                                bullet_mult /= (armorstopvalue * 40);

                                #endregion

                                //Color coolcolor1 = new Color(252, 121, 88);

                                //Color coolcolor2 = new Color(156, 40, 115);

                                ////log.Message("Final chance value based on bullet: ".Colorize(coolcolor1) + bullet_mult.ToString().Colorize(coolcolor2));

                                //log.Message("test 5A5");
                                foreach (BodyPartRecord bodyPart in owan.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside).ToList().FindAll(P => P.def.GetHitChanceFactorFor(DamageDefOf.Cut) > 0))
                                {
                                    if (Rand.Chance(bullet_mult))
                                    {
                                        Hediff lom = HediffMaker.MakeHediff(Caula_DefOf.cut_spall, owan, bodyPart);
                                        lom.Severity = Rand.Range(0.25f, (dinfo.Amount / 6));
                                        owan.health.AddHediff(lom);
                                    }
                                }

                            }

                        }
                    }
                }
                //log.Message("test 6");
            }
            #endregion






            base.PostPreApplyDamage(dinfo, out absorbed);

        }
        #endregion
        public void PostDamageFull(DamageWorker.DamageResult damage)
        {
            Pawn targetPawn = (Pawn)this.parent;
            this.InstestinalSpill(damage);

            if (damage != null)
            {
                if (damage.parts != null)
                {
                    if (damage.parts.Any(rpg => rpg.def == Caula_DefOf.SPinarCord))
                    {
                        Hediff hedf = HediffMaker.MakeHediff(Caula_DefOf.SpinarCordPapasyliz, targetPawn, damage.parts.Find(pp => pp.def == Caula_DefOf.SPinarCord));

                        targetPawn.health.AddHediff(hedf, damage.parts.Find(pp => pp.def == Caula_DefOf.SPinarCord));
                    }
                }
            }


            FractureMeth(damage);
            if (Settings.BruiseStroke)
            {
                Bruise();
            }
           

            if (damage.LastHitPart != null && damage != null) 
            {
                BodyPartRecord dahitpart = damage.LastHitPart;
            }
            
            if (LoadedModManager.GetMod<OofMod>().GetSettings<ToggleSettings>().HydroStaticShockBool)
            {
                if (!damage.diminished && damage != null)
                {
                    if (damage.totalDamageDealt > 31)
                    {
                        if (pope.Def == DamageDefOf.Bullet)
                        {
                            if (Rand.Chance(0.10f))
                            {
                                targetPawn.health.AddHediff(HediffMaker.MakeHediff(Caula_DefOf.hemorrhagicstroke, targetPawn));
                            }
                        }
                    }

                }
            }
            
            List<BodyPartRecord> list = new List<BodyPartRecord>();

            if (Settings.lungcollapse)
            {
                if ( (damage?.parts?.Any(pp => pp.def.defName == "Lung") ?? false) && ((damage?.totalDamageDealt ?? 5) >= 9))
                {
                    if((damDef?.hediff?.injuryProps?.bleedRate ?? 0f) > 0f)
                    {
                        var recorder = damage.parts.Find(pp => pp.def.defName == "Lung");
                        Hediff hediff = HediffMaker.MakeHediff(Caula_DefOf.LungCollapse, targetPawn, recorder);
                        targetPawn.health.AddHediff(hediff);
                    }

                  
                }
            }
           
            
           
        
            Hediff lol = HediffMaker.MakeHediff(HediffDefOf.Stab, targetPawn) as Hediff_Injury;
            
            
            if (LoadedModManager.GetMod<OofMod>().GetSettings<ToggleSettings>().AdrenalineBool)
            {
                DumpAdrenaline(damage.totalDamageDealt);
            }
          

        }
    }
    public class InjuriesCompProps : CompProperties
    {
        public HediffDef Concussion;
        public HediffDef Shock;
        public NeedDef polak;
        

        public InjuriesCompProps()
        {
            this.compClass = typeof(Oof.InjuriesComp);
        }

        public InjuriesCompProps(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }

    }

    [DefOf]
    public static class Caula_DefOf
    {



        
        public static HediffDef Shock;
        public static HediffDef Concussion;
        public static HediffDef adrenalinedump;
        public static HediffDef LungCollapse;
        public static HediffDef hemorrhagicstroke;
        public static BodyPartGroupDef Arms;
        public static HediffDef ChokingOnBlood;
        public static JobDef ClearAirway;
        public static ThingDef suctiondevice;
        public static HediffDef cut_spall;
        public static BodyPartDef SPinarCord;
        public static HediffDef SpinarCordPapasyliz;
        public static HediffDef traumaticbraininjury;
        public static HediffDef EMPTurnOff;
        public static HediffDef Crush;





    }
   
}

