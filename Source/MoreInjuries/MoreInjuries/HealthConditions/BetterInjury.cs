using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions;

public class BetterInjury : Hediff_Injury
{
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

    private float _coagulationMultiplier = 1;
    private int _reducedBleedRateTicksRemaining;
    private int _reducedBleedRateTicksTotal;
    private float _temporarilyTamponadedMultiplierBase = 1;
    private int _coagulationFlags = CoagulationFlag.None;

    public float CoagulationMultiplier
    {
        get => _coagulationMultiplier;
        set => _coagulationMultiplier = value;
    }

    public float EffectiveBleedRateMultiplier
    {
        get
        {
            float multiplier = CoagulationMultiplier;
            if (IsTemporarilyCoagulated)
            {
                multiplier *= TemporarilyTamponadedMultiplier;
            }
            if (IsClosedInternalWound)
            {
                multiplier *= MoreInjuriesMod.Settings.ClosedInternalWouldBleedingModifier;
            }
            return multiplier;
        }
    }

    public CoagulationFlag CoagulationFlags 
    { 
        get => (CoagulationFlag)_coagulationFlags;
        set => _coagulationFlags = value; 
    }

    public int ReducedBleedRateTicksTotal
    {
        get => _reducedBleedRateTicksTotal;
        set
        {
            _reducedBleedRateTicksTotal = value;
            _reducedBleedRateTicksRemaining = value;
        }
    }

    public float TemporarilyTamponadedMultiplierBase 
    { 
        get => _temporarilyTamponadedMultiplierBase; 
        set => _temporarilyTamponadedMultiplierBase = value; 
    }

    public float TemporarilyTamponadedMultiplier => Mathf.Lerp(1, _temporarilyTamponadedMultiplierBase, _reducedBleedRateTicksTotal / _reducedBleedRateTicksRemaining);

    public bool IsTemporarilyCoagulated => !CoagulationFlags.IsEmpty && _reducedBleedRateTicksRemaining > 0 && _reducedBleedRateTicksTotal > 0 && _temporarilyTamponadedMultiplierBase != 1;

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
                    if (injury.CoagulationFlags.IsEmpty && !injury.IsTended())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public override float BleedRate => base.BleedRate * EffectiveBleedRateMultiplier;

    public override string Label
    {
        get
        {
            if (!Bleeding || EffectiveBleedRateMultiplier == 1f)
            {
                return base.Label;
            }
            bool hasPreviousInfo = false;
            StringBuilder builder = new StringBuilder(base.Label)
                .Append(" (");
            if (IsClosedInternalWound)
            {
                builder.AppendInfo("enclosed", ref hasPreviousInfo);
            }
            if (CoagulationFlags.IsSet(CoagulationFlag.Manual))
            {
                builder.AppendInfo("ischemia", ref hasPreviousInfo);
            }
            if (IsTemporarilyCoagulated)
            {
                int durationHours = _reducedBleedRateTicksRemaining / 2500;
                builder.AppendInfo("hemostasis: ", ref hasPreviousInfo)
                    .Append(durationHours)
                    .Append('h');
            }
            builder.Append(')');
            return builder.ToString();
        }
    }

    public override void Tick()
    {
        base.Tick();
        if (!CoagulationFlags.IsEmpty && _reducedBleedRateTicksRemaining > 0)
        {
            _reducedBleedRateTicksRemaining--;
            if (_reducedBleedRateTicksRemaining <= 0)
            {
                // revert the multiplier
                TemporarilyTamponadedMultiplierBase = 1;
                ReducedBleedRateTicksTotal = 0;
                // remove any temporary tamponade flags
                CoagulationFlags = CoagulationFlag.Unset(CoagulationFlags, CoagulationFlag.Timed);
            }
        }
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref _coagulationFlags, "coagulationFlags");
        Scribe_Values.Look(ref _temporarilyTamponadedMultiplierBase, "temporarilyTamponadedMultiplierBase");
        Scribe_Values.Look(ref _coagulationMultiplier, "overriddenBleedRate");
        Scribe_Values.Look(ref _reducedBleedRateTicksTotal, "reducedBleedRateTicksTotal");
        if (_reducedBleedRateTicksRemaining % 300 == 0)
        {
            Scribe_Values.Look(ref _reducedBleedRateTicksRemaining, "reducedBleedRateTicksRemaining", 30000);
        }
        base.ExposeData();
    }

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
            if (!Bleeding || EffectiveBleedRateMultiplier == 1f)
            {
                return base.TipStringExtra;
            }
            StringBuilder builder = new(base.TipStringExtra);
            builder.AppendLine();
            bool hasCustomInfo = false;
            if (IsClosedInternalWound)
            {
                builder.Append("Enclosed internal wound, bleed rate decreased by ")
                    .Append(Math.Round((1f - MoreInjuriesMod.Settings.ClosedInternalWouldBleedingModifier) * 100f, 2))
                    .Append('%')
                    .AppendLine();
                hasCustomInfo = true;
            }
            if (CoagulationFlags.IsSet(CoagulationFlag.Manual))
            {
                builder.Append("Ischemia, bleed rate decreased by ")
                    .Append(Math.Round((1f - CoagulationMultiplier) * 100f, 2))
                    .Append('%')
                    .AppendLine();
                hasCustomInfo = true;
            }
            if (IsTemporarilyCoagulated)
            {
                builder.Append("Hemostasis: ")
                    .Append(_reducedBleedRateTicksRemaining / 2500)
                    .Append("h, bleed rate decreased by ")
                    .Append(Math.Round((1f - TemporarilyTamponadedMultiplier) * 100f, 2))
                    .Append('%')
                    .AppendLine();
                hasCustomInfo = true;
            }
            if (hasCustomInfo)
            {
                builder.AppendLine().AppendLine("Effective bleed rate: ")
                    .Append(Math.Round(EffectiveBleedRateMultiplier * 100f, 2))
                    .Append('%');
            }
            return builder.ToString();
        }
    }
}

file static class StringBuilderExtensions
{
    public static StringBuilder AppendInfo(this StringBuilder builder, string value, ref bool hasPreviousInfo)
    {
        if (hasPreviousInfo)
        {
            builder.Append(", ");
        }
        builder.Append(value);
        hasPreviousInfo = true;
        return builder;
    }
}