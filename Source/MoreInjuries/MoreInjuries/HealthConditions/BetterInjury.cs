using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions;

public class BetterInjury : Hediff_Injury
{
    private static readonly Color s_closedWoundColor = new(115, 115, 115);
    private static readonly Color s_hemostatColor = new(90, 155, 220);

    private static HashSet<string>? s_indirectlyAddedInjuries;

    public static HashSet<string> IndirectlyAddedInjuries
    {
        get
        {
            s_indirectlyAddedInjuries ??=
            [
                KnownHediffDefOf.Fracture.defName,
                KnownHediffDefOf.BoneFragmentLaceration.defName,
                KnownHediffDefOf.SpallFragmentCut.defName
            ];
            return s_indirectlyAddedInjuries;
        }
    }

    private float _overriddenBleedRate;
    private bool _isCoagulationMultiplierApplied = false;
    private int _reducedBleedRateTicks = 30000;
    private int _reducedBleedRateTicksTotal = 30000;
    private bool _isBase = true;

    public float HemostatMultiplier { get; set; }

    public bool IsBase
    {
        get => _isBase;
        set => _isBase = value;
    }

    public float CoagulationMultiplier
    {
        get => _overriddenBleedRate;
        set => _overriddenBleedRate = value;
    }

    public bool IsCoagulationMultiplierApplied
    {
        get => _isCoagulationMultiplierApplied;
        set => _isCoagulationMultiplierApplied = value;
    }

    public int ReducedBleedRateTicksTotal
    {
        get => _reducedBleedRateTicksTotal;
        set
        {
            _reducedBleedRateTicksTotal = value;
            _reducedBleedRateTicks = value;
        }
    }

    public bool IsInternalInjury => Part is { depth: BodyPartDepth.Inside };

    /// <summary>
    /// Returns true if this is an internal injury that is still bleeding and all related external injuries are tended, hemostat applied, or aren't bleeding
    /// </summary>
    public bool IsClosedInternalWound
    {
        get
        {
            // prefer early return
            if (Part is null || !IsInternalInjury || def.injuryProps.bleedRate <= 0 || this.IsPermanent())
            {
                return false;
            }

            // returns true if we are an internal injury and all related external injuries are tended to the best of our ability
            // so Plugged <=> this is an internal injury that is still bleeding and all related external injuries are tended, hemostat applied, or aren't bleeding
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                // must be an external injury that is still bleeding
                if (hediff is BetterInjury { Part.depth: BodyPartDepth.Outside, def.injuryProps.bleedRate: > 0 } injury
                    // must be related to this injury
                    && (injury.Part == Part || injury.Part == Part.parent || injury.Part.parent == Part || injury.Part.parent == Part.parent)
                    // must be tendable now (an active injury)
                    && injury.TendableNow())
                {
                    // if the external injury is still bleeding (not tended), we are not plugged
                    if (!injury.IsCoagulationMultiplierApplied && !injury.IsTended())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public override float BleedRate
    {
        get
        {
            if (IsBase || this.IsTended())
            {
                return base.BleedRate;
            }
            if (IsCoagulationMultiplierApplied)
            {
                return base.BleedRate * CoagulationMultiplier;
            }
            if (IsClosedInternalWound)
            {
                return base.BleedRate * MoreInjuriesMod.Settings.ClosedInternalWouldBleedingModifier;
            }
            Logger.Error($"BetterInjury {def.defName} on {pawn.Name} fell through all cases in BleedRate calculation");
            return base.BleedRate;
        }
    }

    public override string Label
    {
        get
        {
            string result = base.Label;
            if (IsClosedInternalWound)
            {
                result += " (enclosed)";
            }
            if (IsCoagulationMultiplierApplied)
            {
                result += " (tamponaded)";
            }
            return result;
        }
    }

    public override void Tick()
    {
        base.Tick();
        if (IsCoagulationMultiplierApplied && _reducedBleedRateTicks > 0)
        {
            _reducedBleedRateTicks--;
            if (_reducedBleedRateTicks <= 0f)
            {
                IsCoagulationMultiplierApplied = false;
            }
        }
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref _isBase, nameof(_isBase));
        Scribe_Values.Look(ref _isCoagulationMultiplierApplied, nameof(_isCoagulationMultiplierApplied));
        Scribe_Values.Look(ref _overriddenBleedRate, nameof(_overriddenBleedRate));
        Scribe_Values.Look(ref _reducedBleedRateTicksTotal, nameof(_reducedBleedRateTicksTotal));
        if (_reducedBleedRateTicks % 300 == 0)
        {
            Scribe_Values.Look(ref _reducedBleedRateTicks, nameof(_reducedBleedRateTicks), 30000);
        }
        base.ExposeData();
    }

    public override Color LabelColor => (IsCoagulationMultiplierApplied, IsClosedInternalWound) switch
    {
        (true, _) => s_hemostatColor,
        (_, true) => s_closedWoundColor,
        _ => base.LabelColor
    };

    public override void PostAdd(DamageInfo? dinfo)
    {
        // CE isn't playing nicely with us, so we have to do this
        if (Part is { coverageAbs: <= 0f } && (MoreInjuriesMod.CombatExtendedLoaded || IndirectlyAddedInjuries.Contains(def.defName)))
        {
            // these injuries were added by us, so override "Added injury to <part> but it should be impossible to hit" error
            // the correct way to do this would be to transpile the error check in the base implementation, but we don't do that for the following reasons:
            // 1. it's a lot of work for a small gain
            // 2. it may cause incompatibilities with other mods that transpile the same method
            // 3. we can actually abuse the base implementation to achieve the same effect
            // the base implementation throws that error if it thinks the damage def is not supposed to hit the body part
            // but a built-in suppression mechanism exists for the surgical cut damage def
            // so, we abuse that mechanism by setting the damage def to surgical cut, bypassing the error check, and hopefully not having any side effects :fingers_crossed:
            DamageInfo fakeInfo = new
            (
                // the only damage def that bypasses the coverage check is surgical cut
                def: DamageDefOf.SurgicalCut,
                // and choose other parameters to reduce possible side effects as much as possible
                amount: 0,
                instigatorGuilty: false,
                spawnFilth: false,
                checkForJobOverride: false
            );
            dinfo = fakeInfo;
        }
        base.PostAdd(dinfo);
    }

    public override string TipStringExtra
    {
        get
        {
            string result = base.TipStringExtra;
            if (IsClosedInternalWound)
            {
                result += "\nClosed wound, bleeding rate decreased";
            }
            if (IsCoagulationMultiplierApplied)
            {
                result += "\nTemponaded, bleeding rate decreased";
            }
            return result;
        }
    }
}
