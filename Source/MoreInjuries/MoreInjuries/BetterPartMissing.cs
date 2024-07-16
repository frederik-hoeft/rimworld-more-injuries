using Verse;

namespace MoreInjuries;

public class BetterPartMissing : Hediff_MissingPart
{
    public float BleedRateInt = -1f;

    public override float BleedRate
    {
        get
        {
            if (BleedRateInt != -1f && !this.IsTended() && IsFresh)
            {
                return BleedRateInt;
            }
            return base.BleedRate;
        }
    }
}
