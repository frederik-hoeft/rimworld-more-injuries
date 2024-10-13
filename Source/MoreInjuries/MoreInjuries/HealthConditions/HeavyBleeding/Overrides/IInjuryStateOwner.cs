using System.Text;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides;

public interface IInjuryStateOwner
{
    float OverrideEffectiveBleedRateMultiplier(float multiplier);

    string BaseLabel { get; }

    string BaseTipStringExtra { get; }

    void AddCustomLabelAnnotations(StringBuilder builder, ref bool hasPreviousElements);

    void AddCustomTipStringAnnotations(StringBuilder builder, ref bool hasCustomInfo);
}
