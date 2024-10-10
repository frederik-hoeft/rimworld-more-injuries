using Verse;

namespace MoreInjuries.HealthConditions.Amputations;

// TODO: fix this
public class AmputationHediff : Hediff_MissingPart
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
