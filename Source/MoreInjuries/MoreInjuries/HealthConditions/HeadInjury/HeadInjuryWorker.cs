using MoreInjuries.Extensions;
using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeadInjury;

public abstract class HeadInjuryWorker(InjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    private protected const object? __ = null;

    public static HashSet<string> DamageDefNameWhitelist { get; }

    static HeadInjuryWorker()
    {
        // allowed damage types that may cause a hemorrhagic stroke when applied to the head
        DamageDefNameWhitelist =
        [
            "Arrow",
            "ArrowHighVelocity",
            "Beanbag",
            "Bite",
            "BiteToxic",
            DamageDefOf.Blunt.defName,
            DamageDefOf.Bomb.defName,
            "BombSuper",
            DamageDefOf.Bullet.defName,
            "BulletToxic",
            DamageDefOf.Crush.defName,
            "EnergyBolt",
            "Nerve",
            DamageDefOf.Stab.defName,
            "Thermobaric", // from Combat Extended
        ];
    }

    protected abstract float SettingsMaximumEquivalentDamageSkull { get; }

    protected abstract float SettingsChance { get; }

    protected abstract HediffDef HediffDef { get; }

    protected virtual float CalculateSeverityFactor(BodyPartDef bodyPart) => __ switch
    {
        _ when bodyPart == KnownBodyPartDefOf.Brain => 3.0f,
        _ when bodyPart == KnownBodyPartDefOf.Skull => 1.5f,
        _ when bodyPart == KnownBodyPartDefOf.Ear => 1f,
        _ when bodyPart == BodyPartDefOf.Eye => 0.7f,
        _ when bodyPart == KnownBodyPartDefOf.Nose => 0.5f,
        _ => 0.75f
    };

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        if (damage.parts is not List<BodyPartRecord> { Count: > 0 } bodyParts)
        {
            return;
        }
        if (!DamageDefNameWhitelist.Contains(dinfo.Def.defName))
        {
            return;
        }
        Pawn patient = Target;
        float weightedHeadTrauma = 0;
        float aggregatedBodyTrauma = bodyParts.Sum(bodyPart => bodyPart.coverage);
        foreach (BodyPartRecord bodyPart in bodyParts)
        {
            // check if the body part of the head (body part group FullHead)
            if (bodyPart.groups.Contains(BodyPartGroupDefOf.FullHead))
            {
                // determine severity factor based on the specific body part
                float severityFactor = CalculateSeverityFactor(bodyPart.def);
                // calculate the damage to the head based on the severity factor and the weighted damage absorbed by the body part
                weightedHeadTrauma += severityFactor * damage.totalDamageDealt * bodyPart.coverage / aggregatedBodyTrauma;
            }
        }
        if (weightedHeadTrauma == 0f)
        {
            return;
        }
        // HemorrhagicStrokeThreshold is the equivalent damage directly applied to the skull to cause a hemorrhagic stroke
        // and scale everything accordingly to the defined chance
        float chance = Mathf.Clamp01(weightedHeadTrauma / (1.5f * SettingsMaximumEquivalentDamageSkull));
        float adjustedChance = chance * SettingsChance;
        if (Rand.Chance(adjustedChance) && patient.health.hediffSet.GetBrain() is BodyPartRecord brain)
        {
            if (!patient.health.hediffSet.TryGetFirstHediffMatchingPart(brain, HediffDef, out Hediff? stroke))
            {
                stroke = HediffMaker.MakeHediff(HediffDef, patient);
                stroke.Severity = 0.001f;
                patient.health.AddHediff(stroke, brain);
                Logger.LogVerbose($"Added hemorrhagic stroke to {patient.Name}");
            }
            // scale the initial severity of the stroke with quadratic distribution based on the calculated chance
            float severityFactor = Rand.Range(0.01f, Mathf.Max(chance, 0.01f));
            stroke!.Severity += severityFactor * severityFactor;
        }
    }
}
