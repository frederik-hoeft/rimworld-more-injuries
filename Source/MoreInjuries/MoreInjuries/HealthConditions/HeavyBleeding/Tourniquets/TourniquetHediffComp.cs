using MoreInjuries.KnownDefs;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public class TourniquetHediffComp : HediffComp
{
    public List<Hediff_Injury>? Injuries { get; set; }

    public override void CompPostPostRemoved()
    {
        if (Injuries is not null)
        {
            foreach (Hediff_Injury injury in Injuries)
            {
                if (injury is BetterInjury betterInjury)
                {
                    betterInjury.IsBase = true;
                }
            }
        }
        base.CompPostPostRemoved();
    }

    public override void CompPostMake()
    {
        if (parent.Part.def == KnownBodyPartDefOf.Neck)
        {
            Hediff hediff = HediffMaker.MakeHediff(KnownHediffDefOf.ChokingOnTourniquet, parent.pawn);
            parent.pawn.health.AddHediff(hediff);
        }

        base.CompPostMake();
    }
}

