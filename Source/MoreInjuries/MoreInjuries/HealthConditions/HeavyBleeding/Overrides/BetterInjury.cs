using MoreInjuries.Caching;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using MoreInjuries.Localization;
using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides;

public class BetterInjury : Hediff_Injury, IStatefulInjury, IInjuryStateOwner
{
    private static HashSet<HediffDef>? s_indirectlyAddedInjuries;
    private readonly BetterInjuryState<BetterInjury> _state;
    private readonly TimedDataField<BetterInjury, bool, TimedDataEntry<bool>> _isInternalInjuryCache;

    public static HashSet<HediffDef> IndirectlyAddedInjuries => s_indirectlyAddedInjuries ??=
    [
        KnownHediffDefOf.Fracture,
        KnownHediffDefOf.BoneFragmentLaceration,
        KnownHediffDefOf.SpallFragmentCut,
        KnownHediffDefOf.OrganHypoxia,
        KnownHediffDefOf.BrainDamage_Hypoxia,
        KnownHediffDefOf.HemorrhagicStroke,
        KnownHediffDefOf.Bruise,
    ];

    public BetterInjury()
    {
        _state = new BetterInjuryState<BetterInjury>(this);
        _isInternalInjuryCache = new TimedDataField<BetterInjury, bool, TimedDataEntry<bool>>
        (
            owner: this,
            minRefreshIntervalTicks: GenTicks.TickRareInterval,
            dataProvider: static self =>
            {
                // returns true if we are an internal injury and all related external injuries are tended to the best of our ability
                // so Plugged <=> this is an internal injury that is still bleeding and all related external injuries are tended, hemostat applied, or aren't bleeding
                foreach (Hediff hediff in self.pawn.health.hediffSet.hediffs)
                {
                    // must be an external injury that is still bleeding
                    if (hediff is IStatefulInjury injury and HediffWithComps { Part.depth: BodyPartDepth.Outside, def.injuryProps.bleedRate: > 0 }
                        // must be related to this injury
                        && (hediff.Part == self.Part || hediff.Part == self.Part.parent || hediff.Part.parent == self.Part || hediff.Part.parent == self.Part.parent)
                        // must be tendable now (an active injury)
                        && hediff.TendableNow())
                    {
                        // if the external injury is still bleeding (not tended), we are not plugged
                        if (injury.State.CoagulationFlags.IsEmpty && !hediff.IsTended())
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        );
    }

    public IInjuryState State => _state;

    public float OverrideEffectiveBleedRateMultiplier(float multiplier)
    {
        if (IsClosedInternalWound)
        {
            multiplier *= MoreInjuriesMod.Settings.ClosedInternalWouldBleedingModifier;
        }
        return multiplier;
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
            return _isInternalInjuryCache.GetData();
        }
    }

    public bool GetIsClosedInternalWound(bool forceRefresh = false) => _isInternalInjuryCache.GetData(forceRefresh);

    public override float BleedRate => base.BleedRate * _state.EffectiveBleedRateMultiplier;

    public override string Label => _state.Label;

    public string BaseLabel => base.Label;

    public override string TipStringExtra => _state.TipStringExtra;

    public string BaseTipStringExtra => base.TipStringExtra;

    public override void Tick()
    {
        base.Tick();
        _state.Tick();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        _state.ExposeData();
    }

    public override void PostAdd(DamageInfo? dinfo)
    {
        // CE isn't playing nicely with us, so we have to do this
        if (Part is { coverageAbs: <= 0f } && (MoreInjuriesMod.CombatExtendedLoaded || IndirectlyAddedInjuries.Contains(def)))
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
            Logger.Warning($"using legacy compatibility fallback for {this.ToStringSafe()} on {Part.ToStringSafe()}");
        }
        base.PostAdd(dinfo);
    }

    public void AddCustomLabelAnnotations(StringBuilder builder, ref bool hasPreviousInfo)
    {
        if (IsClosedInternalWound)
        {
            builder.AppendEnumerationItem("MI_InjuryEnclosed_Label".Translate(), ref hasPreviousInfo);
        }
    }

    public void AddCustomTipStringAnnotations(StringBuilder builder, ref bool hasCustomInfo)
    {
        if (IsClosedInternalWound)
        {
            double bleedRatePercentage = Math.Round((1f - MoreInjuriesMod.Settings.ClosedInternalWouldBleedingModifier) * 100f, 2);
            builder.AppendLine("MI_InjuryEnclosed_Tooltip".Translate(bleedRatePercentage.Named(Named.Params.PERCENT)));
            hasCustomInfo = true;
        }
    }
}
