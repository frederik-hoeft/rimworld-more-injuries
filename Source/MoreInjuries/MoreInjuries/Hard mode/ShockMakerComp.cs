using Verse;

namespace MoreInjuries;

public class ShockMakerComp : HediffComp
{
    int ticks = 0;

    public bool addedOne = false;

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);

        if (parent.pawn.RaceProps?.Humanlike ?? false)
        {
            ++ticks;
            if (ticks >= 600 && !addedOne)
            {
                if (Rand.Chance(curve.Evaluate(parent.Severity)) && !parent.pawn.health.hediffSet.HasHediff(ShockDefOf.HypovolemicShock))
                {
                    parent.pawn.health.AddHediff(HediffMaker.MakeHediff(ShockDefOf.HypovolemicShock, parent.pawn));
                    addedOne = true;
                }
                ticks = 0;
            }
        }
    }

    public SimpleCurve curve = new(new CurvePoint[]
    {
        new(0f, 0f),
        new(15f, 5f),
        new(50f, 8f),
        new(70f, 10f),
        new(90f, 15f)
    });
}
