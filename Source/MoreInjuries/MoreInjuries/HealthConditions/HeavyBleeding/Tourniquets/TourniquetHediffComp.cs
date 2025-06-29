using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.HeavyBleeding.Overrides;
using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.KnownDefs;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public class TourniquetHediffComp : HediffComp
{
    private const int CHECK_INTERVAL = 300;
    private int _ticksToNextCheck = CHECK_INTERVAL;
    private bool _isGangreneApplied = false; 
    private float _coagulationMultiplier = 1;

    public float CoagulationMultiplier 
    { 
        get => _coagulationMultiplier; 
        set => _coagulationMultiplier = value; 
    }

    public override void CompPostPostRemoved()
    {
        List<Hediff> hediffs = parent.pawn.health.hediffSet.hediffs;
        for (int i = 0; i < hediffs.Count; i++)
        {
            Hediff hediff = hediffs[i];
            if (hediff is IStatefulInjury { State: IInjuryState injuryState } && hediff.IsOnBodyPartOrChildren(parent.Part))
            {
                injuryState.CoagulationFlags &= ~CoagulationFlag.Manual;
                injuryState.CoagulationMultiplier = 1f;
            }
        }
        base.CompPostPostRemoved();
    }

    public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    {
        base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
        ReapplyEffectsToWounds();
    }

    public void ReapplyEffectsToWounds()
    {
        foreach (Hediff hediff in parent.pawn.health.hediffSet.hediffs)
        {
            // tourniquets can only be applied to bleeding injuries that are tendable
            if (hediff is HediffWithComps { Bleeding: true } and IStatefulInjury { State: IInjuryState injuryState }
                && hediff.TendableNow()
                && !injuryState.CoagulationFlags.IsSet(CoagulationFlag.Manual)
                // and the injury must be on the targeted body part or one of its children
                && hediff.IsOnBodyPartOrChildren(parent.Part))
            {
                injuryState.CoagulationFlags |= CoagulationFlag.Manual;
                injuryState.CoagulationMultiplier = CoagulationMultiplier;
            }
        }
    }

    public override void CompExposeData()
    {
        base.CompExposeData();

        Scribe_Values.Look(ref _coagulationMultiplier, "coagulationMultiplier", 1);
        Scribe_Values.Look(ref _ticksToNextCheck, "ticksToNextCheck", CHECK_INTERVAL);
        Scribe_Values.Look(ref _isGangreneApplied, "isGangreneApplied", false);
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);

        if (MoreInjuriesMod.Settings.TourniquetsCanCauseGangrene && !_isGangreneApplied && parent.CurStage.lifeThreatening && --_ticksToNextCheck <= 0)
        {
            _ticksToNextCheck = CHECK_INTERVAL;
            float chance = GetGangreneChance(parent.Severity) * CHECK_INTERVAL / MoreInjuriesMod.Settings.MeanTimeBetweenGangreneOnTourniquet;
            if (Rand.Chance(chance))
            {
                BodyPartRecord? target = GetNextGangreneTargetRandomDepthFirst(parent.pawn, parent.Part);
                if (target is null)
                {
                    // we are done here, no more body parts to apply gangrene to
                    _isGangreneApplied = true;
                    return;
                }
                Hediff gangrene;
                if (Rand.Chance(MoreInjuriesMod.Settings.DryGangreneChance))
                {
                    gangrene = HediffMaker.MakeHediff(KnownHediffDefOf.GangreneDry, parent.pawn, target);
                }
                else
                {
                    gangrene = HediffMaker.MakeHediff(KnownHediffDefOf.GangreneWet, parent.pawn, target);
                }
                if (gangrene.TryGetComp(out HediffComp_CausedBy? causedBy))
                {
                    // the tourniquet is the cause of the gangrene
                    causedBy!.AddCause(parent);
                }
                parent.pawn.health.AddHediff(gangrene, target);
            }
        }
    }

    private static float GetGangreneChance(float severity)
    {
        // follow f(x) = max(32(x - 0.5)^2, 0) for x in [0, 1]
        // which is heavily skewed towards higher values when x approaches 1
        float basis = severity - 0.5f;
        return Mathf.Max(32 * basis * basis, 0f);
    }

    private static BodyPartRecord? GetNextGangreneTargetRandomDepthFirst(Pawn pawn, BodyPartRecord parent)
    {
        if (parent is null || !CanAddGangrene(pawn, parent))
        {
            return null;
        }
        int childCount = parent.parts.Count;
        if (childCount > 0)
        {
            List<BodyPartRecord> children = parent.parts;
            // false = not visited, true = visited
            Span<bool> childPartStatus = stackalloc bool[childCount];
            for (int remaining = childCount; remaining > 0; remaining--)
            {
                int remainingChildIndex = RandomX.Shared.Next(remaining);
                int childIndex = 0;
                for (int i = 0; i < childCount; i++)
                {
                    if (!childPartStatus[i] && remainingChildIndex-- == 0)
                    {
                        childIndex = i;
                        break;
                    }
                }
                BodyPartRecord? child = GetNextGangreneTargetRandomDepthFirst(pawn, children[childIndex]);
                if (child is not null)
                {
                    return child;
                }
                childPartStatus[childIndex] = true;
            }
        }
        return parent;
    }

    private static bool CanAddGangrene(Pawn pawn, BodyPartRecord part) => 
        part is not null
        // exclude arteries and solid parts
        && part.def != KnownBodyPartDefOf.FemoralArtery && part.def != KnownBodyPartDefOf.PoplitealArtery
        && !part.def.IsSolid(part, pawn.health.hediffSet.hediffs)
        && !pawn.health.hediffSet.PartIsMissing(part)
        && !HasGangrene(pawn, part);

    private static bool HasGangrene(Pawn pawn, BodyPartRecord part)
    {
        foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
        {
            if (hediff.Part == part && (hediff.def == KnownHediffDefOf.GangreneDry || hediff.def == KnownHediffDefOf.GangreneWet))
            {
                return true;
            }
        }
        return false;
    }
}