using MoreInjuries.Caching;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

internal sealed class HearingLossWorker(MoreInjuryComp parent, IReadOnlyList<BodyPartGroupDef> earGroups) : InjuryWorker(parent), INotify_UsedVerbHandler
{
    private static readonly WeakTimedDataCache<Pawn, float, HearingLossWorker, TimedDataEntry<float>> s_weakPawnHearingProtectionCache = new
    (
        // we don't expect the apparel to change often, so it's fine if we only refresh it every so often
        minCacheRefreshIntervalTicks: GenTicks.TickLongInterval,
        dataProvider: GetHearingProtectionFactor
    );
    private static readonly Dictionary<ThingDef, bool> s_apparelEarCoverageCache = [];
    private static readonly Dictionary<Type, bool> s_loudVerbTypeCache = [];
    private static HashSet<string>? s_supportedVerbBaseClasses;

    private readonly IReadOnlyList<BodyPartGroupDef> _earGroups = earGroups;

    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableBasicHearingDamage || MoreInjuriesMod.Settings.EnableAdvancedHearingDamage;

    private static HashSet<string> SupportedVerbBaseClasses
    {
        get
        {
            if (s_supportedVerbBaseClasses is not null)
            {
                return s_supportedVerbBaseClasses;
            }
            if (KnownReferenceableDefOf.HearingLossVerbInfo.GetModExtension<HearingLossVerbInfoProperties_ModExtension>() is not { SupportedVerbBaseClasses: [_, ..] supportedVerbBaseClasses })
            {
                Logger.ConfigError("No verb base classes were defined for hearing loss calculation. Check your XML defs.");
                return [];
            }
            return s_supportedVerbBaseClasses = [.. supportedVerbBaseClasses];
        }
    }

    public void Notify_UsedVerb(Pawn pawn, Verb verb)
    {
        Logger.LogDebug($"Used verb {verb.verbProps?.label} ({verb.GetType()}). Pawn: {pawn?.Name}");

        // early exit if the pawn is not equipped with a gun
        if (!IsLoudAction(verb) || verb is not 
        {
            caster: Pawn 
            { 
                Map: not null 
            } shooter, 
            EquipmentSource: { }, 
            verbProps: 
            { 
                muzzleFlashScale: > 0f 
            } gunProperties })
        {
            return;
        }
        // apply hearing damage to the shooter
        ApplyHearingDamage(shooter, shooter);
        // apply hearing damage to nearby pawns if enabled (excluding the shooter)
        if (!MoreInjuriesMod.Settings.EnableAdvancedHearingDamage)
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

    private static float GetHearingProtectionFactor(Pawn pawn, HearingLossWorker state)
    {
        float protectionFactor = 1f;

        // check if anything is worn that covers the ears
        if (pawn.apparel is { WornApparel.Count: > 0 })
        {
            foreach (Apparel clothing in pawn.apparel.WornApparel)
            {
                // cache apparel modifiers independently, since they are fixed and never change
                if (!s_apparelEarCoverageCache.TryGetValue(clothing.def, out bool coversEars))
                {
                    foreach (BodyPartGroupDef earGroup in state._earGroups)
                    {
                        coversEars = clothing.def.apparel.CoversBodyPartGroup(earGroup);
                        if (coversEars)
                        {
                            break;
                        }
                    }
                    s_apparelEarCoverageCache[clothing.def] = coversEars;
                }
                // if the apparel covers the ears, the hearing damage is reduced (stacks with the number of layers)
                if (coversEars)
                {
                    protectionFactor *= 0.05f;
                }
            }
        }
        return protectionFactor;
    }

    private float GetHearingDamageMultiplier(Pawn shooter, Pawn target)
    {
        // scale to the inverse of the distance between the shooter and the target
        float distance = shooter.Position.DistanceTo(target.Position);
        // cap the distance to 0.2f to avoid values approaching infinity
        distance = Mathf.Max(0.2f, distance);
        float result = 1f / distance;

        result *= s_weakPawnHearingProtectionCache.GetData(target, this);

        if (IsIndoors(target))
        {
            // loud noises are more damaging indoors (e.g. gunshots)
            result *= 3;
        }
        return result;
    }

    private static bool IsLoudAction(Verb verb)
    {
        Throw.ArgumentNullException.IfNull(verb);
        Type verbType = verb.GetType();
        if (s_loudVerbTypeCache.TryGetValue(verbType, out bool isLoud))
        {
            return isLoud;
        }
        // lazily traverse the type hierarchy of the verb
        HashSet<string> supportedVerbBaseClasses = SupportedVerbBaseClasses;
        for (Type currentType = verbType; currentType != null; currentType = currentType.BaseType)
        {
            if (supportedVerbBaseClasses.Contains(currentType.FullName))
            {
                Logger.LogDebug($"Verb {verb.verbProps?.label} ({verbType}) is loud. Base class: {currentType.FullName}. Value written to cache.");
                s_loudVerbTypeCache[verbType] = true;
                return true;
            }
        }
        // if no match was found, the verb is not loud
        s_loudVerbTypeCache[verbType] = false;
        Logger.LogDebug($"Verb {verb.verbProps?.label} ({verbType}) is not loud. Value written to cache.");
        return false;
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