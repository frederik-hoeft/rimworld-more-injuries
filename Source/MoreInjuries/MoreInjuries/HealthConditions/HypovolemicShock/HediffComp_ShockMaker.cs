using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

public class HediffComp_ShockMaker : HediffComp
{
    private static readonly SimpleCurve s_curve = new(
    [
        new CurvePoint(0f, 0f),
        // don't apply shock until severity reaches 0.45
        // (otherwise we'll get annoying "medical emergency" messages for minor injuries)
        new CurvePoint(0.45f, 0f),
        new CurvePoint(0.6f, 0.0915f),
        new CurvePoint(1f, 0.1396f),
    ]);

    private bool? _isValid;

    public override void CompPostTick(ref float severityAdjustment)
    {
        Pawn pawn = parent.pawn;
        if (!_isValid.HasValue)
        {
            _isValid = parent.pawn.RaceProps is { Humanlike: true };
        }
        if (!_isValid.Value || !pawn.IsHashIntervalTick(GenTicks.TickRareInterval) 
            || !MoreInjuriesMod.Settings.EnableHypovolemicShock 
            || pawn.health.hediffSet.HasHediff(KnownHediffDefOf.HypovolemicShock))
        {
            return;
        }
        if (Rand.Chance(s_curve.Evaluate(parent.Severity))
            // Biotech integration: don't apply shock if the pawn is deathresting, otherwise leads to infinite hypovolemic shock
            && (!ModLister.BiotechInstalled || !pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest)))
        {
            Hediff hediff = HediffMaker.MakeHediff(KnownHediffDefOf.HypovolemicShock, parent.pawn);
            pawn.health.AddHediff(hediff);
        }
    }
}