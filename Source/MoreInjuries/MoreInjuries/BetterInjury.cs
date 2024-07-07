using UnityEngine;
using Verse;

namespace MoreInjuries;

public class BetterInjury : Hediff_Injury
{
    private static readonly Color _closedWoundColor = new(115, 115, 115);
    private static readonly Color _hemostatColor = new(90, 155, 220);

    private float _overriddenBleedRate;
    private bool _isHemostatApplied = false;
    private int _hemoDuration = 120000;
    private bool _isDiagnosed = false;
    private bool _isBase = true;

    public float HemostatMultiplier { get; set; }

    public bool IsBase 
    { 
        get => _isBase;
        set => _isBase = value;
    }

    public bool IsDiagnosed 
    { 
        get => _isDiagnosed; 
        set => _isDiagnosed = value; 
    }

    public float OverriddenBleedRate 
    { 
        get => _overriddenBleedRate; 
        set => _overriddenBleedRate = value; 
    }

    public bool IsHemostatApplied 
    { 
        get => _isHemostatApplied; 
        set => _isHemostatApplied = value; 
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
                    if (!injury.IsHemostatApplied && !injury.IsTended())
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
            float result;
            if (IsBase || this.IsTended())
            {
                result = base.BleedRate;
            }
            else
            {
                result = OverriddenBleedRate;
            }

            if (IsHemostatApplied)
            {
                result *= HemostatMultiplier;
            }

            if (IsClosedInternalWound)
            {
                result *= MoreInjuriesMod.Settings.PlugMult;
            }

            return result;
        }
    }

    public override void Tick()
    {
        base.Tick();
        if (IsHemostatApplied)
        {
            _hemoDuration--;
            if (_hemoDuration <= 0f)
            {
                IsHemostatApplied = false;
            }
        }
    }

    public override bool Visible
    {
        get
        {
            if (MoreInjuriesMod.Settings.fuckYourFun)
            {
                if (Part?.depth == BodyPartDepth.Outside)
                {
                    return true;
                }

                return IsDiagnosed;
            }
            return true;
        }
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref _isBase, "isBase");
        Scribe_Values.Look(ref _isDiagnosed, "isDiagnosed");
        Scribe_Values.Look(ref _isHemostatApplied, "isHemoStatApplied");
        Scribe_Values.Look(ref _overriddenBleedRate, "BleedRateSet");
        Scribe_Values.Look(ref _hemoDuration, "hemoDuration", 120000);
        base.ExposeData();
    }

    public override Color LabelColor => (IsHemostatApplied, IsClosedInternalWound) switch
    {
        (true, _) => _hemostatColor,
        (_, true) => _closedWoundColor,
        _ => base.LabelColor
    };

    public override string TipStringExtra
    {
        get
        {
            string result = base.TipStringExtra;
            if (IsClosedInternalWound)
            {
                result += "\nClosed wound, bleeding rate decreased";
            }
            if (IsHemostatApplied)
            {
                result += "\nHemostaised, bleeding rate heavily decreased";
            }
            return result;
        }
    }
}
