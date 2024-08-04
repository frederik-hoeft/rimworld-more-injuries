using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.SpallingInjury;

internal class SpallingWorker(InjuryComp parent) : InjuryWorker(parent), IPostPreApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.spall;

    public void PostPreApplyDamage(ref readonly DamageInfo dinfo)
    {
        if (dinfo.Def != DamageDefOf.Bullet)
        {
            return;
        }
        Pawn patient = Target;

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
                    Hediff spall = HediffMaker.MakeHediff(KnownHediffDefOf.SpallFragmentCut, patient, bodyPart);
                    spall.Severity = Rand.Range(0.25f, Mathf.Max(dinfo.Amount / 6f, 0.25f));
                    patient.health.AddHediff(spall);
                }
            }
        }
    }
}

file static class MathEx
{
    public static float Modulo(float a, float b) => a - (b * Mathf.Floor(a / b));
}