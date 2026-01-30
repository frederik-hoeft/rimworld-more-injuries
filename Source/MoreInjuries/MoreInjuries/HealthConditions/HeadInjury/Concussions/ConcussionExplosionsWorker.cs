using MoreInjuries.Defs.WellKnown;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeadInjury.Concussions;

#region Concussion Severity Configuration
// ============================================================================
// CONCUSSION SEVERITY CONFIGURATION - Adjust these values to balance gameplay
// ============================================================================

/// <summary>
/// Body part damage multipliers for concussion generation.
/// Higher values = more concussion damage from the same hit.
/// These represent the "importance" of each brain region:
/// - Head (outer protection): Lowest multiplier, represents deflected/reduced damage
/// - Skull (bone): Medium multiplier, represents structural damage transmission
/// - Brain (core): Highest multiplier, represents direct neural tissue damage
/// </summary>
internal static class ConcussionBodyPartMultipliers
{
    /// <summary>Head - Outer head region, provides some protection. Least severe concussion source.</summary>
    public const float Head = 0.5f;
    
    /// <summary>Skull - Bone structure, medium severity. Impacts can transmit through bone.</summary>
    public const float Skull = 1.5f;
    
    /// <summary>Brain - Direct neural tissue damage, most severe concussion source.</summary>
    public const float Brain = 3.0f;
    
    /// <summary>Default multiplier for unknown/unrecognized head-related body parts.</summary>
    public const float Default = 1.0f;
}

/// <summary>
/// Damage type-to-concussion percentage mapping.
/// Represents how much concussion % is generated per 1 point of actual received damage.
/// Higher values = more concussion per damage.
/// 
/// These percentages represent the traumatic effect of different damage types:
/// - Blunt force (Blunt, Bomb, Thermobaric): High concussion (20-25%)
/// - Penetrating (Bullet, Arrow, Stab): Moderate concussion (5-10%)
/// - Special effects (Crush, Nerve): Varies by mechanism
/// - Other: Conservative default value for unrecognized mod damage types
/// </summary>
internal static class ConcussionDamageTypePercentages
{
    /// <summary>Blunt force trauma - HIGH concussion. Whole-head impact.</summary>
    public const float Blunt = 0.20f; // 20% concussion per 1 damage
    
    /// <summary>Explosive damage - HIGHEST concussion. Blast pressure and fragmentation.</summary>
    public const float Bomb = 0.25f; // 25% concussion per 1 damage
    
    /// <summary>Thermobaric/fuel-air explosives - VERY HIGH concussion. Extreme blast pressure.</summary>
    public const float Thermobaric = 0.25f; // 25% concussion per 1 damage
    
    /// <summary>Bullet impact - LOW concussion. Focused penetrating wound, less whole-head trauma.</summary>
    public const float Bullet = 0.05f; // 5% concussion per 1 damage
    
    /// <summary>Arrow/Bolt impact - LOW concussion. Similar to bullets, penetrating impact.</summary>
    public const float Arrow = 0.05f; // 5% concussion per 1 damage
    
    /// <summary>High-velocity arrows (Combat Extended) - LOW-MODERATE concussion.</summary>
    public const float ArrowHighVelocity = 0.08f; // 8% concussion per 1 damage
    
    /// <summary>Stabbing wound - VERY LOW concussion. Minimal blunt force.</summary>
    public const float Stab = 0.02f; // 2% concussion per 1 damage
    
    /// <summary>Crushing damage - HIGH concussion. Compressive trauma similar to blunt force.</summary>
    public const float Crush = 0.18f; // 18% concussion per 1 damage
    
    /// <summary>Bite damage - MODERATE concussion. Localized impact with some force.</summary>
    public const float Bite = 0.10f; // 10% concussion per 1 damage
    
    /// <summary>Toxic/Caustic damage - LOW concussion. Chemical burns don't cause concussions effectively.</summary>
    public const float Toxic = 0.01f; // 1% concussion per 1 damage
    
    /// <summary>Nerve/EMP damage - MODERATE concussion. Electrical/neural stimulation.</summary>
    public const float Nerve = 0.12f; // 12% concussion per 1 damage
    
    /// <summary>Energy/Plasma bolts - MODERATE-HIGH concussion. Heat + impact.</summary>
    public const float EnergyBolt = 0.15f; // 15% concussion per 1 damage
    
    /// <summary>Beanbag/Non-lethal impact - MODERATE concussion. Designed for blunt trauma.</summary>
    public const float Beanbag = 0.12f; // 12% concussion per 1 damage
    
    /// <summary>Unknown damage types from mods - CONSERVATIVE default.</summary>
    public const float Default = 0.08f; // 8% concussion per 1 damage (conservative middle ground)
}
#endregion

internal sealed class ConcussionExplosionsWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler
{

    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableConcussion;

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Pawn;

        /*
        //original code
        if (dinfo.Def is not null && KnownDamageGroupNames.Explosions.Value.Contains(dinfo.Def.defName))
        {
            // ((1 / e) * x) / ((1 / e) * x + 1)
            const float E_INVERSE = 1f / (float)Math.E;
            float chance = E_INVERSE * dinfo.Amount / ((E_INVERSE * dinfo.Amount) + 1);
            if (Rand.Chance(chance * MoreInjuriesMod.Settings.ConcussionChance) && patient.health.hediffSet.GetBrain() is BodyPartRecord brain)
            {
                if (!patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.Concussion, out Hediff? concussion))
                {
                    concussion = HediffMaker.MakeHediff(KnownHediffDefOf.Concussion, patient);
                    patient.health.AddHediff(concussion, brain);
                }
                // the base severity is a random value between 0 and the initial chance distribution
                // commonly between 0.6 and 0.9, possibly even higher for very high damage
                float baseSeverity = Rand.Range(0f, chance);
                // and now scale all of that logarithmically using
                // f(x)=1/(1+e^(10 * (0.4-x)))
                // such that at an inital chance of 0.8, there is a 50% chance of a severity of above and below 0.5
                // in cases of high damage, the severity will be skewed towards higher values
                float severity = 1f / (1f + Mathf.Exp(10f * (0.4f - baseSeverity)));
                // no clamping required, the function is already bounded between >0.01 and ~0.99
                concussion.Severity = severity;
            }
        }
        */

        // Only process if damage went to a head-related part
        BodyPartRecord? hitPart = dinfo.HitPart;
        if (hitPart is null || !IsHeadPart(hitPart))
        {
            return;
        }
        
        // Get the brain as target for the concussion hediff
        BodyPartRecord? brain = patient.health.hediffSet.GetBrain();
        if (brain is null)
        {
            return;
        }
        
        // Get the damage type concussion percentage
        float concussionPercentPerDamage = GetConcussionPercentageForDamage(dinfo.Def);
        
        // Get the body part multiplier based on the specific hit location
        float bodyPartMultiplier = GetBodyPartMultiplier(hitPart);
        
        // Calculate the percentage of damage that reached this body part
        // This accounts for armor reduction
        float damagePercentageReached = CalculateDamagePercentageReached(dinfo.Amount, hitPart, patient);
        
        // Calculate total concussion severity
        float concussionSeverity = CalculateConcussionSeverity(
            dinfo.Amount,
            damagePercentageReached,
            bodyPartMultiplier,
            concussionPercentPerDamage,
            hitPart
        );
        
        if (concussionSeverity < 0.01f)
        {
            // Ignore negligible concussion
            return;
        }
        
        // Apply or increase concussion hediff
        if (!patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.Concussion, out Hediff? concussion))
        {
            concussion = HediffMaker.MakeHediff(KnownHediffDefOf.Concussion, patient);
            patient.health.AddHediff(concussion, brain);
        }
        
        // Add to existing concussion severity, clamped to max
        concussion.Severity = Mathf.Min(1.0f, concussion.Severity + concussionSeverity);
        
        Logger.LogDebug(
            $"Applied {concussionSeverity:P} concussion to {patient.Name} from {dinfo.Amount:F2} damage " +
            $"to {hitPart.Label} (multiplier: {bodyPartMultiplier}x, damage type: {dinfo.Def?.defName ?? "Unknown"}, " +
            $"percent per dmg: {concussionPercentPerDamage:P}, actual reached: {damagePercentageReached:P})"
        );

    }

    // Determines if a body part is related to the head/brain and should cause concussions.
    private static bool IsHeadPart(BodyPartRecord part)
    {
        // Check if it's the brain, skull, or any part in the head group
        if (part.def == KnownBodyPartDefOf.Brain || 
            part.def == KnownBodyPartDefOf.Skull ||
            part.def == BodyPartDefOf.Head)
        {
            return true;
        }
        
        // Check if it's a child/subpart of the head
        for (BodyPartRecord? current = part.parent; current is not null; current = current.parent)
        {
            if (current.def == BodyPartDefOf.Head)
            {
                return true;
            }
        }
        
        return false;
    }

    // Calculates what percentage of the total damage actually reached the target body part.
    // This accounts for armor reduction by examining the body part's armor values.
    private static float CalculateDamagePercentageReached(float totalDamage, BodyPartRecord targetPart, Pawn pawn)
    {
        if (totalDamage <= 0f)
        {
            return 0f;
        }
        
        // Simple approximation: base damage reduction on body part coverage and armor
        // Higher armor = lower percentage of damage reaches the part
        float armorReduction = 1.0f;
        
        // Account for coverage (covered parts reduce impact damage more)
        if (targetPart.coverage > 0f)
        {
            armorReduction *= targetPart.coverage;
        }
        
        // A pawn with heavy armor takes less concussion damage, but some always gets through
        // This is a simplification - a full implementation would use the damage result
        float minDamagePercentage = 0.1f; // At least 10% of damage always reaches (serious hits)
        float maxDamagePercentage = 1.0f; // At most 100% (no armor)
        
        return Mathf.Clamp(armorReduction, minDamagePercentage, maxDamagePercentage);
    }

    // Gets the body part multiplier based on the specific head region that was hit.
    private static float GetBodyPartMultiplier(BodyPartRecord hitPart)
    {
        // Direct hits to the brain are most severe
        if (hitPart.def == KnownBodyPartDefOf.Brain)
        {
            return ConcussionBodyPartMultipliers.Brain;
        }
        
        // Skull hits are medium severity
        if (hitPart.def == KnownBodyPartDefOf.Skull)
        {
            return ConcussionBodyPartMultipliers.Skull;
        }
        
        // Generic head hits are least severe (dispersed impact)
        if (hitPart.def == BodyPartDefOf.Head)
        {
            return ConcussionBodyPartMultipliers.Head;
        }
        
        // Other head sub-parts (eyes, ears, jaw, etc.) get medium severity
        if (hitPart.parent?.def == BodyPartDefOf.Head || hitPart.parent?.def == KnownBodyPartDefOf.Skull)
        {
            return ConcussionBodyPartMultipliers.Skull;
        }
        
        return ConcussionBodyPartMultipliers.Default;
    }

    // Gets the concussion percentage generation for a specific damage type.
    // Returns the percentage of concussion created per 1 point of damage.
    // Unknown damage types get the default conservative value.
    private static float GetConcussionPercentageForDamage(DamageDef? damageDef)
    {
        if (damageDef is null)
        {
            return ConcussionDamageTypePercentages.Default;
        }
        
        string damageDefName = damageDef.defName;
        
        // Match against known damage types
        return damageDefName switch
        {
            "Blunt" => ConcussionDamageTypePercentages.Blunt,
            "Bomb" => ConcussionDamageTypePercentages.Bomb,
            "BombSuper" => ConcussionDamageTypePercentages.Bomb,
            "Bullet" => ConcussionDamageTypePercentages.Bullet,
            "BulletToxic" => ConcussionDamageTypePercentages.Bullet,
            "Stab" => ConcussionDamageTypePercentages.Stab,
            "Crush" => ConcussionDamageTypePercentages.Crush,
            
            "Arrow" => ConcussionDamageTypePercentages.Arrow,
            "ArrowHighVelocity" => ConcussionDamageTypePercentages.ArrowHighVelocity,
            "Bite" => ConcussionDamageTypePercentages.Bite,
            "BiteToxic" => ConcussionDamageTypePercentages.Toxic,
            "Beanbag" => ConcussionDamageTypePercentages.Beanbag,
            "EnergyBolt" => ConcussionDamageTypePercentages.EnergyBolt,
            "Nerve" => ConcussionDamageTypePercentages.Nerve,
            "Thermobaric" => ConcussionDamageTypePercentages.Thermobaric,
            
            // Unknown damage type - use conservative default
            _ => ConcussionDamageTypePercentages.Default
        };
    }

    // Calculates the final concussion severity based on received damage, body part sensitivity,
    // and damage type, with scaling based on target body part's HP.
    private static float CalculateConcussionSeverity(
        float totalDamage,
        float damagePercentageReached,
        float bodyPartMultiplier,
        float concussionPercentPerDamage,
        BodyPartRecord targetPart)
    {
        // Actual damage that reached this part after armor/coverage reduction
        float actualReceivedDamage = totalDamage * damagePercentageReached;
        
        // Base concussion = damage * damage type percentage * body part multiplier
        float baseConcussion = actualReceivedDamage * concussionPercentPerDamage * bodyPartMultiplier;
        
        // Scale by target body part health - smaller brain = more concussion per damage
        // This makes the same impact more severe on smaller creatures
        float healthScaling = 1.0f;
        if (targetPart.def.hitPoints > 0)
        {
            // Use base hit points from definition (e.g., brain = 10 HP base)
            const float ReferenceHitPoints = 10.0f;
            healthScaling = ReferenceHitPoints / targetPart.def.hitPoints;
        }
        
        float finalConcussion = baseConcussion * healthScaling;
        
        // Clamp to reasonable range
        return Mathf.Clamp(finalConcussion, 0f, 1.0f);
    }

}