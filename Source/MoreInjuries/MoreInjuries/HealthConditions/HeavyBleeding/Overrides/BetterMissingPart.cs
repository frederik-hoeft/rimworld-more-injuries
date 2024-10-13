using System.Text;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides;

public class BetterMissingPart : Hediff_MissingPart, IStatefulInjury, IInjuryStateOwner
{
    private readonly BetterInjuryState<BetterMissingPart> _state;

    public BetterMissingPart()
    {
        _state = new BetterInjuryState<BetterMissingPart>(this);
    }

    public IInjuryState State => _state;

    public override string Label => _state.Label;

    public string BaseLabel => base.Label;

    public override string TipStringExtra => _state.TipStringExtra;

    public string BaseTipStringExtra => base.TipStringExtra;

    public override float BleedRate => base.BleedRate * _state.EffectiveBleedRateMultiplier;

    public override void ExposeData()
    {
        base.ExposeData();
        _state.ExposeData();
    }

    public override void Tick()
    {
        base.Tick();
        _state.Tick();
    }

    public float OverrideEffectiveBleedRateMultiplier(float multiplier) => multiplier;

    public void AddCustomLabelAnnotations(StringBuilder builder, ref bool hasPreviousElements) { }

    public void AddCustomTipStringAnnotations(StringBuilder builder, ref bool hasCustomInfo) { }
}
