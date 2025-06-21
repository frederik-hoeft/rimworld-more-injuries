using Verse;

namespace MoreInjuries.HealthConditions.MechaniteTherapy;

public sealed class HediffComp_MechaniteTherapy : HediffComp
{
    private HediffComp_MechaniteTherapy_OutcomeDoer? _outcomeDoer;

    public HediffCompProperties_MechaniteTherapy Props => (HediffCompProperties_MechaniteTherapy)props;

    public HediffComp_MechaniteTherapy_OutcomeDoer OutcomeDoer
    {
        get => _outcomeDoer ?? throw new InvalidOperationException("OutcomeDoer is not initialized. Ensure it is set before accessing.");
        set => _outcomeDoer = value ?? throw new ArgumentNullException(nameof(value), "OutcomeDoer cannot be null.");
    }

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Deep.Look(ref _outcomeDoer, "outcomeDoer");
    }

    public override void CompPostPostRemoved()
    {
        base.CompPostPostRemoved();
        _outcomeDoer?.DoOutcome(this);
    }
}
