using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse.AI;
using Verse.Sound;
using Verse;
using MoreInjuries.HealthConditions.Choking;
using MoreInjuries.HealthConditions.Fractures;

namespace MoreInjuries;

public class InjuriesComp : ThingComp
{
    #region fields and misc stuff
    public InjuriesCompProps Props => (InjuriesCompProps)props;

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
    {

        Pawn papa = parent as Pawn;
        if (selPawn != papa)
        {
            if (papa.health.hediffSet.hediffs.Any(o => o.def.label == "Heart attack" | o.def == MoreInjuriesHediffDefOf.ChokingOnBlood))
            {
                yield return new FloatMenuOption("Perform CPR", delegate
                {

                    selPawn.jobs.StartJob(new Job(def: DefDatabase<JobDef>.AllDefs.ToList().Find(K => K.defName == "DOCPR"), targetA: papa), JobCondition.InterruptForced);
                });
            }
        }
        if (selPawn.skills.GetSkill(SkillDefOf.Medicine).Level > 0)
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

        Pawn lowan = parent as Pawn;
        //pawns_need.CurLevelPercentage = 0.5f;

        //lowan.needs.TryGetNeed<Need_Adrenaline>.
        base.Initialize(props);
    }

    public void DumpAdrenaline(float DealtDamageChance)
    {
        Pawn lowan = parent as Pawn;

        if (Rand.Chance(DealtDamageChance))
        {
            if (!lowan.health.hediffSet.HasHediff(MoreInjuriesHediffDefOf.adrenalinedump))
            {
                lowan.health.AddHediff(HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.adrenalinedump, lowan));
                Hediff AdrenalineOnPawn = lowan.health.hediffSet.GetFirstHediffOfDef(MoreInjuriesHediffDefOf.adrenalinedump);
                float bloat = Rand.Range(DealtDamageChance * -10f, DealtDamageChance * 2);
                AdrenalineOnPawn.Severity = bloat;

            }
            else
            {
                float bloat = Rand.Range(DealtDamageChance * -10f, DealtDamageChance * 2);
                lowan.health.hediffSet.GetFirstHediffOfDef(MoreInjuriesHediffDefOf.adrenalinedump).Severity += bloat;
            }
        }
    }
    public void Bruise()
    {
        Pawn targetPawn = parent as Pawn;
        List<Hediff> Bruises = targetPawn.health.hediffSet.hediffs.FindAll(Bruise => Bruise.def.defName == "Bruise");
        List<Hediff> SevereBruises = Bruises.FindAll(SevereBruise => SevereBruise.Severity >= 14);
        List<Hediff> LegBruises = Bruises.FindAll(LegBruise => LegBruise.sourceBodyPartGroup == BodyPartGroupDefOf.Legs);
        if (Bruises?.Count > 15 | SevereBruises?.Count > 10 | LegBruises.Count > 5)
        {
            if (Rand.Chance(0.07f))
            {
                targetPawn.health.AddHediff(MoreInjuriesHediffDefOf.hemorrhagicstroke, targetPawn.health.hediffSet.GetBrain());
            }
        }
    }

    public DamageInfo pope;

    public MoreInjuriesSettings Settings
    {
        get
        {
            return LoadedModManager.GetMod<MoreInjuriesMod>().GetSettings<MoreInjuriesSettings>();
        }
    }

    public override void PostExposeData()
    {

        Scribe_Values.Look(ref concussions_suffered, "pawnsufferedthismanyconcussionsusedforTBImechanic");
        base.PostExposeData();
    }

    public void nreTEst(List<BodyPartRecord> dd)
    {
        Pawn targetPawn = (Pawn)parent;
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
        Pawn targetPawn = (Pawn)parent;
        return targetPawn.health.hediffSet.GetNotMissingParts().ToList().FindAll(x => (x.def?.defName ?? "null") == "smolinstestine" | (x.def?.defName ?? "null") == "largeinstestine"
            | (x.def?.defName ?? "null") == "Stomach"
            | (x.def?.defName ?? "null") == "Kidney"
            | (x.def?.defName ?? "null") == "Liver"
        );
    }
    public void InstestinalSpill(DamageWorker.DamageResult damage)
    {
        Pawn targetPawn = (Pawn)parent;
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
        Pawn papa = parent as Pawn;

        #region some choking stuff
        if (Settings.choking)
        {
            foreach (Hediff_Injury injury in papa.health.hediffSet.GetHediffsTendable().OfType<Hediff_Injury>())
            {

                if (injury.Part.def.tags.Contains(BodyPartTagDefOf.BreathingSource) | injury.Part.def.tags.Contains(BodyPartTagDefOf.BreathingPathway) && injury.Bleeding && injury.BleedRate >= 0.20f)
                {
                    if (Rand.Chance(0.70f))
                    {
                        papa.health.AddHediff(MoreInjuriesHediffDefOf.ChokingOnBlood);
                        papa.health.hediffSet.GetFirstHediffOfDef(MoreInjuriesHediffDefOf.ChokingOnBlood).TryGetComp<ChokingHediffComp>().Source = injury;
                        //////
                    }
                }
            }
        }
        #endregion

        #region EMP disabling all bionics
        if (MoreInjuriesMod.Settings.EMPdisablesBionics)
        {
            if (dinfo.Def == DamageDefOf.EMP | dinfo.Def.defName == "Electrical")
            {
                foreach (Hediff part in papa.health.hediffSet.hediffs.FindAll(x => x.def.addedPartProps != null && x.def.addedPartProps.betterThanNatural && x.Part != null))
                {
                    Hediff hediff = HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.EMPTurnOff, papa, part.Part);
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
            if (damage.totalDamageDealt < MoreInjuriesMod.Settings.fractureTreshold)
            {
                return;
            }
            if ((damDef?.armorCategory?.armorRatingStat ?? null) == StatDefOf.ArmorRating_Sharp | (damDef?.armorCategory?.armorRatingStat ?? null) == StatDefOf.ArmorRating_Blunt)
            {
                Pawn targetPawn = (Pawn)parent;
                if (damage?.parts?.Any(x => x.def.IsSolid(x, targetPawn.health.hediffSet.hediffs) && !x.def.IsSkinCovered(x, targetPawn.health.hediffSet) && x.def.bleedRate == 0) ?? false)
                {
                    List<BodyPartRecord> partsbony = damage.parts.FindAll(x => x.def.IsSolid(x, targetPawn.health.hediffSet.hediffs) && !x.def.IsSkinCovered(x, targetPawn.health.hediffSet) && x.def.bleedRate == 0);

                    foreach (BodyPartRecord bone in partsbony)
                    {
                        Hediff fracture = HediffMaker.MakeHediff(DefDatabase<HediffDef>.AllDefs.ToList().Find(x => x.defName == "Fracture"), targetPawn, bone);
                        targetPawn.health.AddHediff(fracture);
                        FractureDefOf.MoreInjuries_BoneSnap.PlayOneShot(new TargetInfo(targetPawn.PositionHeld, targetPawn.Map));
                        if (MoreInjuriesMod.Settings.smolBoniShits)
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

    public Pawn dad => parent as Pawn;

    public void ExtendedExplosion(DamageInfo info)
    {
        if (
            info.Def.defName == "Bomb"
            |
            info.Def.defName == "Thermobaric"
            &&
                false
            )
        {
            IEnumerable<BodyPartRecord> Lungs = dad.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName == "Lung");

            if (!Lungs.EnumerableNullOrEmpty())
            {
                foreach (BodyPartRecord? lung in Lungs)
                {
                    Hediff hediff = HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.Crush, dad, lung);

                    hediff.Severity = Rand.Range(1f, lung.def.hitPoints * 0.75f);

                    dad.health.AddHediff(hediff, lung);
                }
            }
        }
    }

    public void burnLungs(DamageInfo burninfo)
    {
        HediffDef burnHediff = DamageDefOf.Burn.hediff;
        if ((burninfo.Def?.hediff ?? null) == burnHediff)
        {
            IEnumerable<BodyPartRecord> lungs = dad.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName == "Lung");

            foreach (BodyPartRecord? lung in lungs)
            {
                Hediff lungBurnHediff = new() { def = burnHediff, Severity = 200f, Part = lung, pawn = dad };

                if (dad.health.hediffSet.hediffs.Any(x => x.Part?.def?.defName == "Lung" && x.def == burnHediff))
                {

                    List<Hediff> lungburns = dad.health.hediffSet.hediffs.FindAll(x => x.Part?.def?.defName == "Lung" && x.def == burnHediff);

                    foreach (Hediff? idk in lungburns)
                    {
                        idk.Severity += 8f;
                    }
                }
                else
                {

                    dad.health.AddHediff(lungBurnHediff, lung);
                }
            }
        }
    }

    public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        damDef = dinfo.Def;
        Patch_Thing_TakeDamage.Active = true;
        pope = dinfo;
        Pawn papa = parent as Pawn;

        ExtendedExplosion(dinfo);

        touse = dinfo.ArmorPenetrationInt;
        #region spall
        Pawn owan = (Pawn)parent;

        if ((owan.apparel?.WornApparel.Count ?? 0) > 0)
        {

            List<Apparel> armored = owan.apparel.WornApparel?.FindAll(k => k.def.apparel.CoversBodyPart(owan.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined).ToList().Find(P => P.def == BodyPartDefOf.Torso)));
            float touse2 = 0f;

            Apparel aparpub = new();
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

            if (Settings.spall)
            {
                if (dinfo.Def != null && aparpub != null)
                {
                    #region disable spall for cataphracts and similar

                    bool dospall = (aparpub?.def?.techLevel ?? TechLevel.Animal) > TechLevel.Industrial;

                    #endregion

                    #region attempt to make pistols not apply spall with CE
                    bool CE_PistolBool = true;

                    if (ModLister.HasActiveModWithName("Combat Extended"))
                    {
                        if (dinfo.Amount < 13)
                        {
                            CE_PistolBool = false;
                        }
                    }

                    #endregion seems to function alright

                    if (dinfo.Def == DamageDefOf.Bullet && touse < touse2 && CE_PistolBool && dospall)
                    {

                        if (Rand.Chance(Settings.MinSpallHealth - aparpub.HitPoints * 1f / (aparpub.def.BaseMaxHitPoints * 1f)))
                        {
                            /////

                            #region Angle calculations

                            float BulletAngleNinety = dinfo.Angle;

                            if (BulletAngleNinety < 90f)
                            {
                                //
                            }
                            if (BulletAngleNinety > 91f && BulletAngleNinety < 180f)
                            {
                                //
                                BulletAngleNinety -= 90f;
                            }
                            if (BulletAngleNinety > 181f && BulletAngleNinety < 270f)
                            {
                                //
                                BulletAngleNinety -= 180f;
                            }
                            if (BulletAngleNinety > 271f && BulletAngleNinety < 359f)
                            {
                                //
                                BulletAngleNinety -= 270f;

                            }

                            BulletAngleNinety /= 90f;

                            BulletAngleNinety = (float)Math.Round(BulletAngleNinety, 1);

                            //

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

                            bullet_mult *= dinfo.Amount / 18;

                            float armorstopvalue = aparpub.HitPoints * 1f / (aparpub.def.BaseMaxHitPoints * 1f);

                            //

                            bullet_mult /= armorstopvalue * 40;

                            #endregion

                            //Color coolcolor1 = new Color(252, 121, 88);

                            //Color coolcolor2 = new Color(156, 40, 115);

                            //

                            foreach (BodyPartRecord bodyPart in owan.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside).ToList().FindAll(P => P.def.GetHitChanceFactorFor(DamageDefOf.Cut) > 0))
                            {
                                if (Rand.Chance(bullet_mult))
                                {
                                    Hediff lom = HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.cut_spall, owan, bodyPart);
                                    lom.Severity = Rand.Range(0.25f, dinfo.Amount / 6);
                                    owan.health.AddHediff(lom);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        base.PostPreApplyDamage(ref dinfo, out absorbed);
    }
    #endregion
    public void PostDamageFull(DamageWorker.DamageResult damage)
    {
        Pawn targetPawn = (Pawn)parent;
        InstestinalSpill(damage);

        if (damage != null)
        {
            if (damage.parts != null)
            {
                if (damage.parts.Any(rpg => rpg.def == MoreInjuriesHediffDefOf.SPinarCord))
                {
                    Hediff hedf = HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.SpinarCordPapasyliz, targetPawn, damage.parts.Find(pp => pp.def == MoreInjuriesHediffDefOf.SPinarCord));

                    targetPawn.health.AddHediff(hedf, damage.parts.Find(pp => pp.def == MoreInjuriesHediffDefOf.SPinarCord));
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

        if (LoadedModManager.GetMod<MoreInjuriesMod>().GetSettings<MoreInjuriesSettings>().HydroStaticShockBool)
        {
            if (!damage.diminished && damage != null)
            {
                if (damage.totalDamageDealt > 31)
                {
                    if (pope.Def == DamageDefOf.Bullet)
                    {
                        if (Rand.Chance(0.10f))
                        {
                            targetPawn.health.AddHediff(HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.hemorrhagicstroke, targetPawn));
                        }
                    }
                }
            }
        }

        List<BodyPartRecord> list = new();

        if (Settings.lungcollapse)
        {
            if ((damage?.parts?.Any(pp => pp.def.defName == "Lung") ?? false) && (damage?.totalDamageDealt ?? 5) >= 9)
            {
                if ((damDef?.hediff?.injuryProps?.bleedRate ?? 0f) > 0f)
                {
                    BodyPartRecord recorder = damage.parts.Find(pp => pp.def.defName == "Lung");
                    Hediff hediff = HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.LungCollapse, targetPawn, recorder);
                    targetPawn.health.AddHediff(hediff);
                }
            }
        }
        Hediff lol = HediffMaker.MakeHediff(DamageDefOf.Stab.hediff, targetPawn) as Hediff_Injury;

        if (LoadedModManager.GetMod<MoreInjuriesMod>().GetSettings<MoreInjuriesSettings>().AdrenalineBool)
        {
            DumpAdrenaline(damage.totalDamageDealt);
        }
    }
}

