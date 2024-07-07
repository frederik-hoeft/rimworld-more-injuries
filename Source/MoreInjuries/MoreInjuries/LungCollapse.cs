using System.Linq;
using Verse;

namespace MoreInjuries;

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
        if (pawn.health.hediffSet.hediffs.Any(x => x.def.defName == "AirwayBlocked"))
        {
            pawn.health.RemoveHediff(pawn.health.hediffSet.hediffs.Find(x => x.def.defName == "AirwayBlocked"));
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
                BodyPartRecord? neck = pawn.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName == "Neck").FirstOrFallback();
                Hediff hediff = HediffMaker.MakeHediff(InjuryDefOf.AirwayBlocked, pawn, neck);

                pawn.health.AddHediff(hediff, neck);
            }
        }
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref IsFresh, "IsFresh");
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
