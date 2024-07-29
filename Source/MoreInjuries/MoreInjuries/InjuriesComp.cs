using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse.AI;
using Verse.Sound;
using Verse;
using MoreInjuries.HealthConditions.Choking;
using MoreInjuries.HealthConditions.Fractures;
using MoreInjuries.Bcl;
using MoreInjuries.Debug;
using UnityEngine;

namespace MoreInjuries;

// TODO: split into classes and namespaces
public class InjuriesComp : ThingComp
{
    // TODO: do we need a Postfix patch? (if not, remove these)
    private DamageDef? _damageDef;
    private DamageInfo _damageInfo;

    // Choking + Fractures
    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectedPawn)
    {
        Pawn patient = (Pawn)parent;
        // can't perform CPR on yourself, that would be pretty alpha
        if (!ReferenceEquals(selectedPawn, patient))
        {
            if (patient.health.hediffSet.hediffs.Any(PerformCprJob.CanBeTreatedWithCpr))
            {
                yield return new FloatMenuOption("Perform CPR", () => selectedPawn.jobs.StartJob(new Job(def: MoreInjuriesJobDefOf.PerformCpr, targetA: patient), JobCondition.InterruptForced));
            }
        }
        if (selectedPawn.skills.GetSkill(SkillDefOf.Medicine).Level > 0 
            && patient.health.hediffSet.hediffs.Any(hediff => hediff.def == FractureDefOf.Fracture) 
            && selectedPawn.inventory.innerContainer.Any(thing => thing.def == MoreInjuriesThingDefOf.Splint))
        {
            yield return new FloatMenuOption("Fix fracture", () => selectedPawn.jobs.StartJob(new Job(def: MoreInjuriesJobDefOf.ApplySplint, targetA: patient), JobCondition.InterruptForced));
        }
    }

    // Adrenaline Dump
    public void DumpAdrenaline(float totalDamageDealt)
    {
        if (!MoreInjuriesMod.Settings.UseAdrenaline)
        {
            return;
        }
        Pawn patient = (Pawn)parent;
        if (Rand.Chance(totalDamageDealt))
        {
            if (!patient.health.hediffSet.TryGetHediff(MoreInjuriesHediffDefOf.AdrenalineDump, out Hediff? adrenalineDump))
            {
                // add new hediff
                adrenalineDump = HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.AdrenalineDump, patient);
                adrenalineDump.Severity = 0;
                patient.health.AddHediff(adrenalineDump);
            }
            // TODO: negative severity?
            float severity = Rand.Range(totalDamageDealt * -10f, totalDamageDealt * 2);
            adrenalineDump.Severity += severity;
        }
    }

    // Hemorrhagic Stroke
    public void ApplyHemorrhagicStroke()
    {
        // TODO: rename
        if (!MoreInjuriesMod.Settings.BruiseStroke)
        {
            return;
        }
        Pawn patient = (Pawn)parent;
        int totalBruises = 0;
        int severeBruises = 0;
        int legBruises = 0;

        foreach (Hediff hediff in patient.health.hediffSet.hediffs)
        {
            if (hediff.def.defName is HediffDefNameOf.Bruise)
            {
                totalBruises++;
                if (hediff.Severity >= 14)
                {
                    severeBruises++;
                }
                if (hediff.sourceBodyPartGroup == BodyPartGroupDefOf.Legs)
                {
                    legBruises++;
                }
            }
        }
        // TODO: add a setting for the chance
        // apply hemorrhagic stroke if any of the thresholds are met
        if ((totalBruises > 16 || severeBruises > 8 || legBruises > 4) && Rand.Chance(0.07f))
        {
            patient.health.AddHediff(MoreInjuriesHediffDefOf.HemorrhagicStroke, patient.health.hediffSet.GetBrain());
        }
    }

    // Intestinal Spill
    private void InstestinalSpill(DamageWorker.DamageResult damage)
    {
        Pawn targetPawn = (Pawn)parent;
        if (damage.parts is null)
        {
            return;
        }

        if (damage.parts.Any(bodyPart => 
            bodyPart.def == KnownBodyPartDefOf.SmallIntestine 
            || bodyPart.def == KnownBodyPartDefOf.LargeIntestine 
            || bodyPart.def == KnownBodyPartDefOf.Stomach))
        {
            ReadOnlySpan<BodyPartDef> affectedOrgans =
            [
                KnownBodyPartDefOf.SmallIntestine,
                KnownBodyPartDefOf.LargeIntestine,
                KnownBodyPartDefOf.Stomach,
                KnownBodyPartDefOf.Kidney,
                KnownBodyPartDefOf.Liver
            ];

            foreach (BodyPartRecord bodyPart in targetPawn.health.hediffSet.GetNotMissingParts())
            {
                // if we have spillage from the intestines and any of the affected organs are bleeding, there's a chance to cause acid burns
                if (affectedOrgans.Contains(bodyPart.def)
                    && targetPawn.health.hediffSet.hediffs.Any(hediff => hediff is { Part: not null, Bleeding: true } && hediff.Part == bodyPart) 
                    && Rand.Chance(0.45f))
                {
                    Hediff burn = HediffMaker.MakeHediff(InjuryDefOf.StomachAcidBurn, targetPawn, bodyPart);
                    burn.Severity = Rand.Range(1, 7f);
                    targetPawn.health.AddHediff(burn);
                }
            }
        }
    }

    // Choking
    private void Choke()
    {
        if (!MoreInjuriesMod.Settings.ChokingEnabled)
        {
            return;
        }
        Pawn patient = (Pawn)parent;
        foreach (Hediff_Injury injury in patient.health.hediffSet.GetHediffsTendable().OfType<Hediff_Injury>())
        {
            if (injury is { Bleeding: true, BleedRate: >= 0.20f }
                && injury.Part.def.tags.Any(tag => tag == BodyPartTagDefOf.BreathingSource || tag == BodyPartTagDefOf.BreathingPathway) 
                && Rand.Chance(0.70f))
            {
                Hediff choking = HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.ChokingOnBlood, patient);
                if (choking.TryGetComp(out ChokingHediffComp? comp))
                {
                    comp!.Source = injury;
                    patient.health.AddHediff(choking);
                    return;
                }
                Log.Error("Failed to get ChokingHediffComp from choking hediff");
            }
        }
    }

    // EMP Damage to Bionics
    private void DisableBionicsFromEmp(ref readonly DamageInfo dinfo)
    {
        if (!MoreInjuriesMod.Settings.EMPdisablesBionics)
        {
            return;
        }
        Pawn patient = (Pawn)parent;
        
        if (dinfo.Def == DamageDefOf.EMP || dinfo.Def == DamageDefOf.ElectricalBurn)
        {
            foreach (Hediff part in patient.health.hediffSet.hediffs.Where(hediff => hediff is { Part: not null, def.addedPartProps.betterThanNatural: true } ))
            {
                Hediff hediff = HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.EmpShutdown, patient, part.Part);
                hediff.Severity = 1f;
                patient.health.AddHediff(hediff, part.Part);
            }
        }
    }

    public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    {
        Choke();
        DisableBionicsFromEmp(in dinfo);

        base.PostPostApplyDamage(dinfo, totalDamageDealt);
    }

    // Fractures
    private void Fracture(DamageWorker.DamageResult damage)
    {
        if (!MoreInjuriesMod.Settings.toggleFractures || damage.totalDamageDealt < MoreInjuriesMod.Settings.fractureTreshold)
        {
            return;
        }

        StatDef? armorRatingStat = _damageDef?.armorCategory?.armorRatingStat;
        if (armorRatingStat != StatDefOf.ArmorRating_Sharp && armorRatingStat != StatDefOf.ArmorRating_Blunt)
        {
            // unable to apply unknown damage type
            return;
        }

        Pawn patient = (Pawn)parent;

        if (damage.parts is not null)
        {
            // get all solid body parts that received a non-bleeding injury
            // (note from maintainer: no idea why we are checking for skin coverage)
            IEnumerable<BodyPartRecord> affectedBones = damage.parts.Where(bodyPart => 
                bodyPart.def.IsSolid(bodyPart, patient.health.hediffSet.hediffs) 
                && !bodyPart.def.IsSkinCovered(bodyPart, patient.health.hediffSet) 
                && bodyPart.def.bleedRate == 0);

            foreach (BodyPartRecord bone in affectedBones)
            {
                Hediff fracture = HediffMaker.MakeHediff(FractureDefOf.Fracture, patient, bone);
                patient.health.AddHediff(fracture);
                FractureDefOf.MoreInjuries_BoneSnap.PlayOneShot(new TargetInfo(patient.PositionHeld, patient.Map));
                if (MoreInjuriesMod.Settings.UseBoneFragmentLacerations)
                {
                    foreach (BodyPartRecord sibling in bone.parent.GetDirectChildParts())
                    {
                        // TODO: add a setting for the chance
                        if (Rand.Chance(0.10f))
                        {
                            Hediff shards = HediffMaker.MakeHediff(FractureDefOf.BoneFragmentLaceration, patient, sibling);
                            shards.Severity = Rand.Range(1f, 5f);
                            patient.health.AddHediff(shards);
                        }
                        foreach (BodyPartRecord child in sibling.GetDirectChildParts())
                        {
                            if (Rand.Chance(0.10f))
                            {
                                Hediff shards = HediffMaker.MakeHediff(FractureDefOf.BoneFragmentLaceration, patient, child);
                                shards.Severity = Rand.Range(1f, 5f);
                                patient.health.AddHediff(shards);
                            }
                        }
                    }
                }
            }
        }
    }

    // Lung Collapse / Thermobaric damage
    private void CollapseLungs(ref readonly DamageInfo info)
    {
        if (!MoreInjuriesMod.Settings.lungcollapse)
        {
            return;
        }
        Pawn patient = (Pawn)parent;
        if (info.Def == DamageDefOf.Bomb || info.Def.defName is "Thermobaric")
        {
            IEnumerable<BodyPartRecord> lungs = patient.health.hediffSet.GetNotMissingParts().Where(bodyPart => bodyPart.def == BodyPartDefOf.Lung);

            foreach (BodyPartRecord lung in lungs)
            {
                Hediff hediff = HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.LungCollapse, patient, lung);
                hediff.Severity = Rand.Range(1f, Mathf.Max(lung.def.hitPoints * 0.75f, 1f));
                patient.health.AddHediff(hediff, lung);
            }
        }
    }

    // Thermal Inhalation Injury
    private void BurnLungs(ref readonly DamageInfo burninfo)
    {
        if (!MoreInjuriesMod.Settings.enableFireInhalation)
        {
            return;
        }
        HediffDef burnHediffDef = DamageDefOf.Burn.hediff;
        Pawn patient = (Pawn)parent;
        if (burninfo.Def.hediff == burnHediffDef)
        {
            IEnumerable<BodyPartRecord> lungs = patient.health.hediffSet.GetNotMissingParts().Where(bodyPart => bodyPart.def == BodyPartDefOf.Lung);
            foreach (BodyPartRecord lung in lungs)
            {
                bool hasBurnedLung = false;
                // get burn injuries on that lung
                foreach (Hediff lungBurn in patient.health.hediffSet.hediffs.Where(hediff => hediff.Part == lung && hediff.def == burnHediffDef))
                {
                    hasBurnedLung = true;
                    lungBurn.Severity += 8f;
                }
                if (!hasBurnedLung)
                {
                    Hediff lungBurn = HediffMaker.MakeHediff(burnHediffDef, patient, lung);
                    lungBurn.Severity = 200f;
                    patient.health.AddHediff(lungBurn, lung);
                }
            }
        }
    }

    // Spalling
    private void Spall(ref readonly DamageInfo dinfo)
    {
        if (!MoreInjuriesMod.Settings.spall || dinfo.Def != DamageDefOf.Bullet)
        {
            return;
        }
        Pawn patient = (Pawn)parent;

        // if there is no armor there is nothing for the bullet to fragment off of (quick check)
        if (patient.apparel?.WornApparel.Count is not > 0)
        {
            return;
        }

        IEnumerable<Apparel> armoredVests = patient.apparel.WornApparel.Where(apparel => apparel.def.apparel.CoversBodyPart(patient.health.hediffSet.GetBodyPartRecord(BodyPartDefOf.Torso)));

        // get the best armor currently worn
        float maxArmorRating = 0f;
        Apparel? bestArmor = null;
        foreach (Apparel armor in armoredVests)
        {
            if (armor.GetStatValue(StatDefOf.ArmorRating_Sharp) > maxArmorRating)
            {
                maxArmorRating = armor.GetStatValue(StatDefOf.ArmorRating_Sharp);
                bestArmor = armor;
            }
        }
        // if there is no armor there or the armor is too weak to stop the bullet, then there is nothing for the bullet to fragment off of (slow check)
        if (bestArmor?.def is null || dinfo.ArmorPenetrationInt >= maxArmorRating)
        {
            return;
        }
        // disable spall for cataphracts and similar
        if (bestArmor.def.techLevel > TechLevel.Industrial)
        {
            return;
        }
        // if combat extended is loaded, disable spall for low damage bullets (approximation)
        if (MoreInjuriesMod.CombatExtendedLoaded)
        {
            if (dinfo.Amount < 13)
            {
                return;
            }
        }

        float chance = MoreInjuriesMod.Settings.MinSpallHealth - bestArmor.HitPoints * 1f / (bestArmor.def.BaseMaxHitPoints * 1f);
        if (Rand.Chance(chance))
        {
            // likelihood of spall increases as the angle of the bullet approaches 90 degrees (perpendicular impact)
            // for narrow angles, the bullet is more likely to be deflected and less energy is transferred to deformation and fragmentation
            // normalize the angle to the range [0, 180)
            float normalizedAngle = MathEx.Modulo(Mathf.Abs(dinfo.Angle), 180f);
            // normalize to the range [0, 90] (of any side)
            if (normalizedAngle > 90f)
            {
                normalizedAngle = 180f - normalizedAngle;
            }
            // Calculate the bullet multiplier as a percentage between straight on and 90 degrees, in 1% increments
            // We want the multiplier to be 1 at 90 degrees and 0 at 0 degrees
            float bulletMultiplier = (float)Math.Round(normalizedAngle / 90f, 2);
            // apply random magic factor to the multiplier (not sure why)
            bulletMultiplier *= dinfo.Amount / 18f;
            // apply armor thickness to the multiplier
            float armorStopValue = bestArmor.HitPoints * 1f / (bestArmor.def.BaseMaxHitPoints * 1f);
            bulletMultiplier /= armorStopValue * 40f;

            // apply spall to all body parts that can be cut, according to the hit chance factor
            foreach (BodyPartRecord bodyPart in patient.health.hediffSet.GetNotMissingParts(depth: BodyPartDepth.Outside).Where(bodyPart => bodyPart.def.GetHitChanceFactorFor(DamageDefOf.Cut) > 0))
            {
                if (Rand.Chance(bulletMultiplier))
                {
                    Hediff spall = HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.CutSpall, patient, bodyPart);
                    spall.Severity = Rand.Range(0.25f, Mathf.Max(dinfo.Amount / 6f, 0.25f));
                    patient.health.AddHediff(spall);
                }
            }
        }
    }

    public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        _damageDef = dinfo.Def;
        Patch_Thing_TakeDamage.IsActive = true;
        _damageInfo = dinfo;

        CollapseLungs(in dinfo);
        BurnLungs(in dinfo);
        Spall(in dinfo);

        base.PostPreApplyDamage(ref dinfo, out absorbed);
    }

    // Spinal Cord Paralysis
    private void Paralyze(DamageWorker.DamageResult damage)
    {
        if (damage.parts is null)
        {
            return;
        }
        Pawn patient = (Pawn)parent;
        if (damage.parts.FirstOrDefault(rpg => rpg.def == MoreInjuriesHediffDefOf.SpinalCord) is BodyPartRecord spinalCord)
        {
            Hediff paralysis = HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.SpinalCordParalysis, patient, spinalCord);
            patient.health.AddHediff(paralysis, spinalCord);
        }
    }

    // Hydrostatic Shock
    // TODO: controversial
    private void ApplyHydrostaticShock(DamageWorker.DamageResult damage)
    {
        if (!MoreInjuriesMod.Settings.UseHydrostaticShock)
        {
            return;
        }
        Pawn patient = (Pawn)parent;
        if (!damage.diminished && damage.totalDamageDealt > 31 && _damageInfo.Def == DamageDefOf.Bullet)
        {
            if (Rand.Chance(0.10f))
            {
                patient.health.AddHediff(HediffMaker.MakeHediff(MoreInjuriesHediffDefOf.HemorrhagicStroke, patient));
            }
        }
    }

    public void PostDamageFull(DamageWorker.DamageResult damage)
    {
        DebugAssert.NotNull(damage, "damage is null in PostDamageFull");

        InstestinalSpill(damage);
        Paralyze(damage);
        Fracture(damage);
        ApplyHemorrhagicStroke();
        ApplyHydrostaticShock(damage);
        DumpAdrenaline(damage.totalDamageDealt);
    }
}

file static class MathEx
{
    public static float Modulo(float a, float b) => a - (b * Mathf.Floor(a / b));
}