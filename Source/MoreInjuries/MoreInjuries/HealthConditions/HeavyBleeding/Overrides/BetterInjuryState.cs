using MoreInjuries.Extensions;
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
                builder.AppendEnumerationItem("ischemia", ref hasPreviousElements);
            }
            if (IsTemporarilyCoagulated)
            {
                int durationHours = _reducedBleedRateTicksRemaining / 2500;
                builder.AppendEnumerationItem("hemostasis: ", ref hasPreviousElements)
                    .Append(durationHours)
                    .Append('h');
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
            owner.AddCustomLabelAnnotations(builder, ref hasCustomInfo);
            if (hasCustomInfo)
            {
                builder.AppendLine().AppendLine("Effective bleed rate: ")
                    .Append(Math.Round(EffectiveBleedRateMultiplier * 100f, 2))
                    .Append('%');
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