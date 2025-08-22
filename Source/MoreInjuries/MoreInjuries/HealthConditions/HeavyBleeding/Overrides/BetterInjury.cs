using MoreInjuries.Caching;
using MoreInjuries.Extensions;
using MoreInjuries.Localization;
using RimWorld;
using System.Text;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides;

public class BetterInjury : Hediff_Injury, IStatefulInjury, IInjuryStateOwner
{
    private readonly BetterInjuryState<BetterInjury> _state;
    private readonly TimedDataField<BetterInjury, bool, TimedDataEntry<bool>> _isInternalInjuryCache;

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
