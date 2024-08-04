using MoreInjuries.KnownDefs;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

public class ShockMakerHediffComp : HediffComp
{
    private static readonly SimpleCurve s_curve = new(
    [
        new(0f, 0f),
        new(15f, 5f),
        new(50f, 8f),
        new(70f, 10f),
        new(90f, 15f)
    ]);

    private int _ticks = 0;
    private bool _hasShock = false;

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);

        if (parent.pawn.RaceProps?.Humanlike is true)
        {
            if (_ticks < 600)
            {
                _ticks++;
                return;
            }
            if (!_hasShock)
            {
                if (Rand.Chance(s_curve.Evaluate(parent.Severity)) && !parent.pawn.health.hediffSet.HasHediff(KnownHediffDefOf.HypovolemicShock))
                {
                    parent.pawn.health.AddHediff(HediffMaker.MakeHediff(KnownHediffDefOf.HypovolemicShock, parent.pawn));
                    _hasShock = true;
                }
                _ticks = 0;
            }
        }
    }
}