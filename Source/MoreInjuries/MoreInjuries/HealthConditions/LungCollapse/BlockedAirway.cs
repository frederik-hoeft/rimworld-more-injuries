using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse;

public class BlockedAirway : BetterInjury
{
    private bool _isFresh = true;
    private int _ticks = 600;

    public override float BleedRate => 0f;

    public override bool TendableNow(bool ignoreTimer = false)
    {
        return _isFresh;
    }

    public override void Tended(float quality, float maxQuality, int batchPosition = 0)
    {
        _isFresh = false;
        if (pawn.health.hediffSet.hediffs.Any(x => x.def.defName == "AirwayBlocked"))
        {
            pawn.health.RemoveHediff(pawn.health.hediffSet.hediffs.Find(x => x.def.defName == "AirwayBlocked"));
        }
        base.Tended(quality, maxQuality, batchPosition);
    }

    public override void Tick()
    {
        _ticks--;
        if (_ticks >= 0)
        {
            OnTimerElapsed();
            _ticks = 600;
        }
        base.Tick();
    }

    public virtual void OnTimerElapsed()
    {
        if (_isFresh && Rand.Chance(0.05f))
        {
            BodyPartRecord? neck = pawn.health.hediffSet.GetNotMissingParts().FirstOrDefault(x => x.def.defName is BodyPartDefNameOf.Neck)
                // honestly, the neck should never be missing, but just in case
                ?? throw new InvalidOperationException("Cannot apply airway blockage to a pawn without a neck. What's going on here?");
            Hediff hediff = HediffMaker.MakeHediff(InjuryDefOf.AirwayBlocked, pawn, neck);
            pawn.health.AddHediff(hediff, neck);
        }
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref _isFresh, "IsFresh");
    }

    public override string TipStringExtra
    {
        get
        {
            if (_isFresh)
            {
                return "Fresh. Airways in danger.";
            }

            return "Tended. Airways cleared. Awaiting surgery.";
        }
    }
}
