using MoreInjuries.Extensions;
using MoreInjuries.Localization;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides;

public class BetterInjuryState<TOwner>(TOwner owner) : IExposable, IInjuryState where TOwner : HediffWithComps, IInjuryStateOwner
{
    private int _coagulationFlags;
    private float _coagulationMultiplier = 1;
    private int _reducedBleedRateTicksRemaining;
    private int _reducedBleedRateTicksTotal;
    private float _temporarilyTamponadedMultiplierBase = 1;

    public CoagulationFlag CoagulationFlags
    {
        get => (CoagulationFlag)_coagulationFlags;
        set => _coagulationFlags = value;
    }

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
            return owner.OverrideEffectiveBleedRateMultiplier(multiplier);
        }
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

    public string Label
    {
        get
        {
            if (!owner.Bleeding || EffectiveBleedRateMultiplier == 1f)
            {
                return owner.BaseLabel;
            }
            bool hasPreviousElements = false;
            StringBuilder builder = new StringBuilder(owner.BaseLabel)
                .Append(" (");
            if (CoagulationFlags.IsSet(CoagulationFlag.Manual))
            {
                builder.AppendEnumerationItem("MI_InjuryBleeding_ManuallyReduced_Label".Translate(), ref hasPreviousElements);
            }
            if (IsTemporarilyCoagulated)
            {
                int durationHours = _reducedBleedRateTicksRemaining / 2500;
                builder.AppendEnumerationItem(
                    "MI_InjuryBleeding_TemporarilyReduced_Label".Translate(
                        Named.Keys.Format_TimeHours
                            .Translate(durationHours.Named(Named.Params.HOURS))
                            .Named(Named.Params.TIME)), ref hasPreviousElements);
            }
            owner.AddCustomLabelAnnotations(builder, ref hasPreviousElements);
            builder.Append(')');
            return builder.ToString();
        }
    }

    public string TipStringExtra
    {
        get
        {
            if (!owner.Bleeding || EffectiveBleedRateMultiplier == 1f)
            {
                return owner.BaseTipStringExtra;
            }
            StringBuilder builder = new(owner.BaseTipStringExtra);
            builder.AppendLine();
            bool hasCustomInfo = false;
            if (CoagulationFlags.IsSet(CoagulationFlag.Manual))
            {
                double bleedRateDecrease = Math.Round((1f - CoagulationMultiplier) * 100f, 2);
                builder.AppendLine("MI_InjuryBleeding_ManuallyReduced_Tooltip".Translate(bleedRateDecrease.Named(Named.Params.PERCENT)));
                hasCustomInfo = true;
            }
            if (IsTemporarilyCoagulated)
            {
                int durationHours = _reducedBleedRateTicksRemaining / 2500;
                double bleedRateDecrease = Math.Round((1f - TemporarilyTamponadedMultiplier) * 100f, 2);
                builder.AppendLine("MI_InjuryBleeding_TemporarilyReduced_Tooltip".Translate(
                    Named.Keys.Format_TimeHours
                        .Translate(durationHours.Named(Named.Params.HOURS))
                        .Named(Named.Params.TIME),
                    bleedRateDecrease.Named(Named.Params.PERCENT)));
                hasCustomInfo = true;
            }
            owner.AddCustomLabelAnnotations(builder, ref hasCustomInfo);
            if (hasCustomInfo)
            {
                double effectiveBleedRate = Math.Round(EffectiveBleedRateMultiplier * 100f, 2);
                builder.AppendLine()
                    .AppendLine("MI_InjuryBleeding_EffectiveBleedRate_Tooltip"
                        .Translate(effectiveBleedRate.Named(Named.Params.PERCENT)));
            }
            return builder.ToString();
        }
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref _coagulationFlags, "coagulationFlags");
        Scribe_Values.Look(ref _temporarilyTamponadedMultiplierBase, "temporarilyTamponadedMultiplierBase");
        Scribe_Values.Look(ref _coagulationMultiplier, "overriddenBleedRate");
        Scribe_Values.Look(ref _reducedBleedRateTicksTotal, "reducedBleedRateTicksTotal");
        if (_reducedBleedRateTicksRemaining % 300 == 0)
        {
            Scribe_Values.Look(ref _reducedBleedRateTicksRemaining, "reducedBleedRateTicksRemaining", 30000);
        }
    }

    public void Tick()
    {
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
}