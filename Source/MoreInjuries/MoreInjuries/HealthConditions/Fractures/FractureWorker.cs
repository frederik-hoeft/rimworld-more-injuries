﻿using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace MoreInjuries.HealthConditions.Fractures;

internal class FractureWorker(InjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler, ICompFloatMenuOptionsHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableFractures;

    public IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectedPawn)
    {
        Pawn patient = Target;
        if (selectedPawn.skills.GetSkill(SkillDefOf.Medicine).Level > 0
            && patient.health.hediffSet.hediffs.Any(hediff => hediff.def == KnownHediffDefOf.Fracture)
            && selectedPawn.inventory.innerContainer.Any(thing => thing.def == KnownThingDefOf.Splint))
        {
            return
            [
                new FloatMenuOption("Fix fracture", () => selectedPawn.jobs.StartJob(new Job(def: KnownJobDefOf.ApplySplint, targetA: patient), JobCondition.InterruptForced))
            ];
        }
        return [];
    }

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        if (damage.totalDamageDealt < MoreInjuriesMod.Settings.FractureDamageTreshold)
        {
            return;
        }

        StatDef? armorRatingStat = dinfo.Def?.armorCategory?.armorRatingStat;
        if (armorRatingStat != StatDefOf.ArmorRating_Sharp && armorRatingStat != StatDefOf.ArmorRating_Blunt)
        {
            // unable to apply unknown damage type
            return;
        }

        Pawn patient = Target;

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
                if (!Rand.Chance(MoreInjuriesMod.Settings.FractureChanceOnDamage))
                {
                    continue;
                }
                Hediff fracture = HediffMaker.MakeHediff(KnownHediffDefOf.Fracture, patient, bone);
                patient.health.AddHediff(fracture);
                KnownSoundDefOf.MoreInjuries_BoneSnap.PlayOneShot(new TargetInfo(patient.PositionHeld, patient.Map));
                if (MoreInjuriesMod.Settings.EnableBoneFragmentLacerations)
                {
                    foreach (BodyPartRecord sibling in bone.parent.GetDirectChildParts())
                    {
                        // TODO: add a setting for the chance
                        if (Rand.Chance(0.10f))
                        {
                            Hediff shards = HediffMaker.MakeHediff(KnownHediffDefOf.BoneFragmentLaceration, patient, sibling);
                            shards.Severity = Rand.Range(1f, 5f);
                            patient.health.AddHediff(shards);
                        }
                        foreach (BodyPartRecord child in sibling.GetDirectChildParts())
                        {
                            if (Rand.Chance(0.10f))
                            {
                                Hediff shards = HediffMaker.MakeHediff(KnownHediffDefOf.BoneFragmentLaceration, patient, child);
                                shards.Severity = Rand.Range(1f, 5f);
                                patient.health.AddHediff(shards);
                            }
                        }
                    }
                }
            }
        }
    }
}