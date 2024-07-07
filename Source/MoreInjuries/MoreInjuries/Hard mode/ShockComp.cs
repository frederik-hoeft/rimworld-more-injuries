using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MoreInjuries;

public class ShockComp : HediffComp
{
    public ShockCompProps Props => props as ShockCompProps;

    public bool PastFixedPoint
    {
        get
        {
            return parent.Severity > 0.6f;
        }
    }

    public bool fixedNow = false;

    public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
    {
        base.CompTended(quality, maxQuality, batchPosition);
        if (quality >= Props.BleedSeverityCurve.Evaluate(parent.Severity))
        {
            fixedNow = true;
        }
    }

    public Hediff BloodLoss
    {
        get
        {
            return parent.pawn.health.hediffSet.hediffs.Find(x => x.def == HediffDefOf.BloodLoss);
        }
    }

    public int ticks = 0;

    public override void CompPostTick(ref float severityAdjustment)
    {
        ticks++;
        base.CompPostTick(ref severityAdjustment);
        if ((BloodLoss == null && !PastFixedPoint) | fixedNow)
        {
            //this.parent.pawn.health.RemoveHediff(this.parent);
            parent.Severity -= 0.000025f;
        }
        if (!fixedNow)
        {
            if (!PastFixedPoint)
            {
                parent.Severity = BloodLoss.Severity;
            }
            else
            {
                parent.Severity += 0.00005f;

                if (ticks >= 300)
                {
                    if (Rand.Chance(MoreInjuriesMod.Settings.hypoxiaChance))
                    {
                        BodyPartRecord part = InternalBps.Where(x => x.def.bleedRate > 0f).RandomElement();

                        Hediff hediff = HediffMaker.MakeHediff(ShockDefOf.InternalSuffocation, parent.pawn, part);

                        hediff.Severity = Rand.Range(2f, 5f);

                        parent.pawn.health.AddHediff(hediff, part);

                        ticks = 0;
                    }
                }
            }
        }
    }

    public IEnumerable<BodyPartRecord> InternalBps
    {
        get
        {
            return parent.pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Middle, BodyPartDepth.Inside).Where(x => x.def != BodyPartDefOf.Heart);
        }
    }
}
