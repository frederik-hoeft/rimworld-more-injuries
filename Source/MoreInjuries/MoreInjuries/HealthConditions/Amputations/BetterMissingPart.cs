using Verse;

namespace MoreInjuries.HealthConditions.Amputations;

public class BetterMissingPart : Hediff_MissingPart
{
    public float BleedRateInt { get; set; } = -1f;

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
