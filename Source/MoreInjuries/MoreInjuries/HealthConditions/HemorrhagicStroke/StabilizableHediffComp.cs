using Verse;

namespace MoreInjuries.HealthConditions.HemorrhagicStroke;

public class StabilizableHediffComp : HediffComp
{
    private bool _stabilized;

    public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
    {
        base.CompTended(quality, maxQuality, batchPosition);
        if (quality > Rand.Range(0.7f, 0.9f))
        {
            _stabilized = true;
            parent.Severity = Math.Min(0.15f, parent.Severity);
        }
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (_stabilized)
        {
            severityAdjustment = 0.00001f;
        }
        base.CompPostTick(ref severityAdjustment);
    }
}
