﻿using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Fractures.Lacerations;
using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.Things;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace MoreInjuries.HealthConditions.Fractures;

internal sealed class FractureWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler, ICompFloatMenuOptionsHandler
{
    private static readonly Dictionary<BodyPartDef, ILacerationHandler> s_lacerationRegistry;

    static FractureWorker()
    {
        (BodyPartDef def, ILacerationHandler handler)[] lacerationRegistry =
        [
            // head
            (KnownBodyPartDefOf.Skull, new SiblingsAndDecendantsLacerationHandler()),           // any soft tissue in the head
            (KnownBodyPartDefOf.Jaw, new SiblingsAndDecendantsLacerationHandler(targets:        // nearby soft tissue
            [
                KnownBodyPartDefOf.Ear,
                KnownBodyPartDefOf.Nose,
                KnownBodyPartDefOf.Tongue
            ])),
            (KnownBodyPartDefOf.Nose, new SelfLacerationHandler()),                             // nose
            // arms
            (KnownBodyPartDefOf.Clavicle, new ParentLacerationHandler()),                       // shoulder
            (KnownBodyPartDefOf.Humerus, new ParentLacerationHandler()),                        // arm 
            (KnownBodyPartDefOf.Radius, new ParentLacerationHandler()),                         // arm
            (BodyPartDefOf.Arm, new SelfLacerationHandler()),                                   // arm
            (BodyPartDefOf.Hand, new SelfAndDescendantsLacerationHandler(targets:               // hand + fingers
            [
                KnownBodyPartDefOf.Finger
            ])),
            (KnownBodyPartDefOf.Finger, new SelfLacerationHandler()),                           // finger
            // torso
            (KnownBodyPartDefOf.Ribcage, new SiblingLacerationHandler(targets:                  // nearby soft tissue
            [
                BodyPartDefOf.Lung,
                BodyPartDefOf.Heart,
                KnownBodyPartDefOf.Liver,
                KnownBodyPartDefOf.Stomach,
                KnownBodyPartDefOf.LargeIntestine,
                KnownBodyPartDefOf.Kidney,
            ])),
            (KnownBodyPartDefOf.Sternum, new SiblingLacerationHandler(targets:                  // nearby soft tissue
            [
                BodyPartDefOf.Heart,
                BodyPartDefOf.Lung
            ])),
            (KnownBodyPartDefOf.Spine, new DescendantLacerationHandler()),                      // spinal cord
            (KnownBodyPartDefOf.Pelvis, new FirstSiblingAndDecendantsLacerationHandler(targets: // nearby soft tissue
            [
                KnownBodyPartDefOf.Abdomen,
                KnownBodyPartDefOf.SmallIntestine,
                KnownBodyPartDefOf.LargeIntestine
            ])),
            // legs
            (BodyPartDefOf.Leg, new SelfAndDescendantsLacerationHandler(targets:                // nearby soft tissue
            [
                KnownBodyPartDefOf.FemoralArtery,
                KnownBodyPartDefOf.PoplitealArtery
            ])),
            (KnownBodyPartDefOf.Femur, new ParentAndSiblingsLacerationHandler(targets:          // nearby soft tissue
            [
                BodyPartDefOf.Leg,
                KnownBodyPartDefOf.FemoralArtery
            ])),
            (KnownBodyPartDefOf.Tibia, new ParentAndSiblingsLacerationHandler(targets:          // nearby soft tissue
            [
                BodyPartDefOf.Leg,
                KnownBodyPartDefOf.PoplitealArtery
            ])),
            (KnownBodyPartDefOf.Foot, new SelfAndDescendantsLacerationHandler(targets:          // foot + toes
            [
                KnownBodyPartDefOf.Toe
            ])),
            (KnownBodyPartDefOf.Toe, new SelfLacerationHandler())                               // toe
        ];
        s_lacerationRegistry = lacerationRegistry.ToDictionary(static pair => pair.def, static pair => pair.handler);
    }

    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableFractures;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = Target;
        if (!builder.Keys.Contains(UITreatmentOption.UseSplint) && selectedPawn.Drafted && patient.health.hediffSet.hediffs.Any(static hediff => hediff.def == KnownHediffDefOf.Fracture))
        {
            builder.Keys.Add(UITreatmentOption.UseSplint);
            if (!KnownResearchProjectDefOf.BasicAnatomy.IsFinished)
            {
                return;
            }
            if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_UseSplint.JOB_LABEL_KEY) is { FailureReason: string failure })
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.Splint, JobDriver_UseSplint.TargetHediffDefs) is not Thing thing)
            {
                builder.Options.Add(new FloatMenuOption("MI_UseSplintFailed_Unavailable".Translate(JobDriver_UseSplint.JOB_LABEL_KEY.Translate()), null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_UseSplint.JOB_LABEL_KEY.Translate(), JobDriver_UseSplint.GetDispatcher(selectedPawn, patient, thing).StartJob));
            }
        }
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

        if (damage.parts is not null && patient is { Dead: false } && patient.Map is Map map)
        {
            // get all breakable body parts that received damage
            IEnumerable<BodyPartRecord> affectedBones = damage.parts.Where(bodyPart =>
                s_lacerationRegistry.ContainsKey(bodyPart.def) 
                && !patient.health.hediffSet.PartIsMissing(bodyPart));

            foreach (BodyPartRecord bone in affectedBones)
            {
                if (!Rand.Chance(MoreInjuriesMod.Settings.FractureChanceOnDamage))
                {
                    continue;
                }
                // the body part shouldn't be already broken
                if (patient.health.hediffSet.TryGetFirstHediffMatchingPart(bone, KnownHediffDefOf.Fracture, out _))
                {
                    continue;
                }
                Hediff fracture = HediffMaker.MakeHediff(KnownHediffDefOf.Fracture, patient, bone);
                patient.health.AddHediff(fracture);
                KnownSoundDefOf.BoneSnap.PlayOneShot(new TargetInfo(patient.PositionHeld, map));
                if (MoreInjuriesMod.Settings.EnableBoneFragmentLacerations && Rand.Chance(MoreInjuriesMod.Settings.SplinteringFractureChance))
                {
                    IEnumerable<BodyPartRecord> lacerationTargets = s_lacerationRegistry[bone.def].GetTargets(patient, bone);
                    float chance = MoreInjuriesMod.Settings.BoneFragmentLacerationChancePerBodyPart;
                    foreach (BodyPartRecord lacerationTarget in lacerationTargets)
                    {
                        if (Rand.Chance(chance))
                        {
                            Hediff shards = HediffMaker.MakeHediff(KnownHediffDefOf.BoneFragmentLaceration, patient, lacerationTarget);
                            // severity is random between 0 and 5, but with a curve towards lower values
                            float curve = Rand.Range(0f, 1f);
                            shards.Severity = curve * curve * 5f;
                            if (shards.TryGetComp(out HediffComp_CausedBy? causedBy))
                            {
                                causedBy!.AddCause(fracture);
                            }
                            patient.health.AddHediff(shards);
                        }
                    }
                }
            }
        }
    }
}