using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

public class HediffComp_ShockMaker : HediffComp
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

        if (MoreInjuriesMod.Settings.EnableHypovolemicShock && parent.Severity >= 0.45f && parent.pawn.RaceProps is { Humanlike: true })
        {
            if (_hasShock || ++_ticks < 600)
            {
                return;
            }
            _ticks = 0;
            if (Rand.Chance(s_curve.Evaluate(parent.Severity)) && !parent.pawn.health.hediffSet.HasHediff(KnownHediffDefOf.HypovolemicShock)
                // Biotech integration: don't apply shock if the pawn is deathresting, otherwise leads to infinite hypovolemic shock
                && (!ModLister.BiotechInstalled || !parent.pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest)))
            {
                parent.pawn.health.AddHediff(HediffMaker.MakeHediff(KnownHediffDefOf.HypovolemicShock, parent.pawn));
                _hasShock = true;
            }
        }
    }
}