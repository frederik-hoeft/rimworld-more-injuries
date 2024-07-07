using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Oof
{
    [StaticConstructorOnStartup]
    public class InjuryClassChanger
    {
        static InjuryClassChanger()
        {
            foreach (HediffDef hediffdef in DefDatabase<HediffDef>.AllDefsListForReading.FindAll(t => t.hediffClass == typeof(Hediff_Injury)))
            {
                //log.Message("changed hediffclass of " + hediffdef.label.Colorize(new Color(100, 100, 0)) + " to BetterInjury"); 
                hediffdef.hediffClass = typeof(BetterInjury);
            }
            foreach (HediffDef hediffdef in DefDatabase<HediffDef>.AllDefsListForReading.FindAll(t => t.hediffClass == typeof(Hediff_MissingPart)))
            {
                hediffdef.hediffClass = typeof(BetterPartMissing);
            }
        }
    }

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
                    result *= OofMod.settings.PlugMult;
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
                if (OofMod.settings.fuckYourFun)
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

    public class LungCollapse : BetterInjury
    {
        public bool IsFresh = true;

        public override float BleedRate => 0f;

        public override bool TendableNow(bool ignoreTimer = false)
        {
            return IsFresh;
        }

        public override void Tended(float quality, float maxQuality, int batchPosition = 0)
        {
            IsFresh = false;
            if (this.pawn.health.hediffSet.hediffs.Any(x => x.def.defName == "AirwayBlocked"))
            {
                this.pawn.health.RemoveHediff(this.pawn.health.hediffSet.hediffs.Find(x => x.def.defName == "AirwayBlocked"));
            }
            base.Tended(quality, maxQuality, batchPosition);
        }


        public int ticks = 600;
        public override void Tick()
        {
            ticks--;
            if (ticks >= 0)
            {
                TenSeconds();
                ticks = 600;
            }
            base.Tick();
        }

        public virtual void TenSeconds()
        {
            if (IsFresh)
            {
                if (Rand.Chance( /*0.003f*/ 1f))
                {
                    var neck = this.pawn.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName == "Neck").FirstOrFallback();
                    var hediff = HediffMaker.MakeHediff(InjuryDefOf.AirwayBlocked, this.pawn, neck);

                    this.pawn.health.AddHediff(hediff, neck);
                }
            }

        }

        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref IsFresh, "IsFresh");
        }

        public override TextureAndColor StateIcon => base.StateIcon;

        public override string TipStringExtra
        {
            get
            {
                if (IsFresh)
                {
                    return "Fresh. Airways in danger.";
                }

                return "Tended. Airways cleared. Awaiting surgery.";
            }
        }
    }

    public class BetterPartMissing : Hediff_MissingPart
    {
        public float BleedRateInt = -1f;
        public override float BleedRate
        {
            get
            {
                if (BleedRateInt != -1f && !this.IsTended() && this.IsFresh)
                {
                    return BleedRateInt;
                }
                return base.BleedRate;
            }
        }
    }


    [DefOf]
    public class InjuryDefOf
    {
        public static HediffDef AirwayBlocked;
    }
}
