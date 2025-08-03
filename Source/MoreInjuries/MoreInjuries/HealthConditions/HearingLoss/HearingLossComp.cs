using System.Collections.Generic;
using System.Linq;
using MoreInjuries.Defs.WellKnown;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

public class HearingLossComp : ThingComp
{
    private static readonly Dictionary<string, bool> s_apparelEarCoverageCache = [];
    private static BodyPartGroupDef[]? s_earGroups = null;

    public float GetHearingDamageMultiplier(Pawn shooter, Pawn target)
    {
        // scale to the inverse of the distance between the shooter and the target
        float distance = shooter.Position.DistanceTo(target.Position);
        // cap the distance to 0.2f to avoid values approaching infinity
        distance = Mathf.Max(0.2f, distance);
        float result = 1f / distance;
        // check if anything is worn that covers the ears
        if (target.apparel is { WornApparel.Count: > 0 })
        {
            foreach (Apparel clothing in target.apparel.WornApparel)
            {
                if (!s_apparelEarCoverageCache.TryGetValue(clothing.def.defName, out bool coversEars))
                {
                    if (s_earGroups is null)
                    {
                        Logger.LogDebug($"attempting to generate ear group cache from pawn {target.Name}");
                        // not yet cached, check if the apparel covers the ears
                        BodyPartRecord? ear = target.health.hediffSet.GetNotMissingParts().FirstOrDefault(static bodyPart => bodyPart.def == KnownBodyPartDefOf.Ear);
                        if (ear is null)
                        {
                            // pawn has no ears, no need to check further
                            Logger.Log($"{target.Name} has no ears, delaying cache generation");
                            break;
                        }
                        s_earGroups = [.. ear.groups];
                        Logger.LogDebug($"successfully initialized ear group cache with {s_earGroups.Length} groups");
                    }
                    // get stack-local reference to allow the JIT to optimize checks
                    BodyPartGroupDef[] earGroups = s_earGroups!;
                    for (int i = 0; i < earGroups.Length && !coversEars; i++)
                    {
                        coversEars = clothing.def.apparel.CoversBodyPartGroup(earGroups[i]);
                    }
                    s_apparelEarCoverageCache[clothing.def.defName] = coversEars;
                }
                // if the apparel covers the ears, the hearing damage is reduced (stacks with the number of layers)
                if (coversEars)
                {
                    result *= 0.05f;
                }
            }
        }

        if (IsIndoors(target))
        {
            // loud noises are more damaging indoors (e.g. gunshots)
            result *= 3;
        }
        return result;
    }

    public override void Notify_UsedWeapon(Pawn shooter)
    {
        bool advancedHearingDamage = MoreInjuriesMod.Settings.EnableAdvancedHearingDamage;
        if (!MoreInjuriesMod.Settings.EnableBasicHearingDamage && !advancedHearingDamage)
        {
            return;
        }
        // early exit if the pawn is not a human
        if (shooter.def != ThingDefOf.Human)
        {
            return;
        }
        // early exit if the pawn is not equipped with a gun
        if (shooter.equipment?.Primary?.TryGetComp<CompEquippable>()?.PrimaryVerb.verbProps is not VerbProperties { range: > 0f, muzzleFlashScale: > 0f } gunProperties)
        {
            return;
        }
        // exit early if the map is null
        if (shooter.Map is null)
        {
            return;
        }
        // apply hearing damage to the shooter
        ApplyHearingDamage(shooter, shooter);
        // apply hearing damage to nearby pawns if enabled (excluding the shooter)
        if (!advancedHearingDamage)
        {
            return;
        }
        float radius = Mathf.Min(gunProperties.muzzleFlashScale, 3f);
        // when shot indoors, the gunshot sound is damaging over a larger area
        if (IsIndoors(shooter))
        {
            radius *= 1.25f;
        }
        // get all pawns in the vicinity of the shooter and apply hearing damage
        IEnumerable<IntVec3> cellsInVicinity = GenRadial.RadialCellsAround(shooter.Position, radius, useCenter: true);
        foreach (IntVec3 cell in cellsInVicinity)
        {
            if (!cell.InBounds(shooter.Map))
            {
                continue;
            }
            List<Thing> pawnsInCell = shooter.Map.thingGrid.ThingsListAtFast(cell);
            // cannot use iterator here because apparently the list is concurrently modified (sketch!)
            for (int i = 0; i < pawnsInCell.Count; i++)
            {
                if (pawnsInCell[i] is Pawn otherPawn && shooter != otherPawn)
                {
                    ApplyHearingDamage(shooter, otherPawn);
                }
            }
        }
    }

    private void ApplyHearingDamage(Pawn shooter, Pawn otherPawn)
    {
        float hearingDamageMultiplier = GetHearingDamageMultiplier(shooter, otherPawn);
        if (Rand.Chance(hearingDamageMultiplier / 10f))
        {
            if (!otherPawn.health.hediffSet.TryGetHediff(KnownHediffDefOf.HearingLossTemporary, out Hediff? hearingLoss))
            {
                hearingLoss = HediffMaker.MakeHediff(KnownHediffDefOf.HearingLossTemporary, otherPawn);
                otherPawn.health.AddHediff(hearingLoss);
            }
            // scale by the hearing damage multiplier and the mod settings factor
            hearingLoss.Severity += hearingDamageMultiplier / 100f * MoreInjuriesMod.Settings.HearingDamageTemporarySeverityFactor;
            HearingLossHelper.TryMakePermanentIfApplicable(otherPawn, hearingLoss);
        }
    }

    private static bool IsIndoors(Pawn pawn) => !pawn.Position.UsesOutdoorTemperature(pawn.Map);
}