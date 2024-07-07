using Verse;

namespace MoreInjuries;

public class tendFixerComp : HediffComp
{
    public bool SemiFixed = false;
    public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
    {
        base.CompTended(quality, maxQuality, batchPosition);
        if (quality > 0.8f)
        {
            SemiFixed = true;
            parent.Severity = Math.Min(0.15f, parent.Severity);
        }
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (SemiFixed)
        {
            severityAdjustment = 0.00001f;
        }
        base.CompPostTick(ref severityAdjustment);

    }
}
