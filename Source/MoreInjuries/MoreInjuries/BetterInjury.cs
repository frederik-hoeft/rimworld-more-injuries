using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries;

public class BetterInjury : Hediff_Injury
{
    public bool isBase = true;

    public float BleedRateSet;

    public float hemoMult;

    public bool hemod = false;

    public int hemoDuration = 120000;

    public bool AmIInternal
    {
        get
        {
            return (this.Part?.depth ?? BodyPartDepth.Undefined) == BodyPartDepth.Inside;
        }
    }

    public bool Plugged
    {
        get
        {
            if (this.Part != null)
            {
                var injuryList = this.pawn.health.hediffSet.GetInjuriesTendable().Where(x => x.Part.depth == BodyPartDepth.Outside && x is BetterInjury).Select(y => y as BetterInjury);

                injuryList = injuryList.Where(x => !x.IsPermanent() && x.def.injuryProps.bleedRate > 0 && x.Part == this.Part | x.Part == this.Part?.parent | x.Part?.parent == this.Part | x.Part?.parent == this.Part?.parent);

                return injuryList.All(x => (x.IsTended() | x.hemod)) && AmIInternal && !this.IsPermanent() && this.def.injuryProps.bleedRate > 0;
            }
            return false;
        }
    }

    public override float BleedRate
    {
        get
        {
            float result = 0f;
            if (isBase | this.IsTended())
            {
                result = base.BleedRate;
            }
            else
            {
                result = BleedRateSet;
            }

            if (hemod)
            {
                result *= hemoMult;
            }

            if (Plugged && AmIInternal)
            {
                result *= MoreInjuriesMod.Settings.PlugMult;
            }

            return result;
        }
    }

    public override void Tick()
    {
        base.Tick();
        if (hemod)
        {
            hemoDuration--;
            if (hemoDuration <= 0f)
            {
                hemod = false;
            }
        }
    }

    public bool diagnosed = false;
    public override bool Visible
    {
        get
        {
            if (MoreInjuriesMod.Settings.fuckYourFun)
            {
                if (this.Part?.depth == BodyPartDepth.Outside)
                {
                    return true;
                }

                return diagnosed;
            }
            return true;
        }
    }

    public override void ExposeData()
    {
        Scribe_Values.Look<bool>(ref isBase, "isBased");
        Scribe_Values.Look<bool>(ref diagnosed, "diagnose");
        Scribe_Values.Look<bool>(ref hemod, "diagnose");
        Scribe_Values.Look<float>(ref BleedRateSet, "BleedRateSet");
        Scribe_Values.Look<int>(ref hemoDuration, "hemoDuration", 120000);
        base.ExposeData();
    }

    public override Color LabelColor
    {
        get
        {
            if (hemod)
            {
                return new Color(90, 155, 220);
            }

            if (Plugged)
            {
                return new Color(115, 115, 115);
            }
            return base.LabelColor;
        }
    }

    public override string TipStringExtra
    {
        get
        {
            string result = base.TipStringExtra;
            if (Plugged)
            {
                result += "\nClosed wound, bleeding rate decreased";
            }
            if (hemod)
            {
                result += "\nHemostaised, bleeding rate heavily decreased";
            }
            return result;
        }
    }
}
