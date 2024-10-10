using MoreInjuries.HealthConditions.HeavyBleeding.Bandages;
using MoreInjuries.HealthConditions.HeavyBleeding.HemostaticAgents;
using MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding;

public class HeavyBleedingWorker : InjuryWorker, ICompFloatMenuOptionsHandler, ICompGetGizmosExtraHandler
{
    private readonly ICompFloatMenuOptionsHandler[] _childHandlers;

    public HeavyBleedingWorker(MoreInjuryComp parent) : base(parent)
    {
        _childHandlers =
        [
            new BandageFloatOptionProvider(this),
            new HemostaticAgentFloatOptionProvider(this),
            new TourniquetFloatOptionProvider(this)
        ];
    }

    public override bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        foreach (ICompFloatMenuOptionsHandler handler in _childHandlers)
        {
            handler.AddFloatMenuOptions(builder, selectedPawn);
        }
    }

    public void AddGizmosExtra(UIBuilder<Gizmo> builder, Pawn selectedPawn)
    {
        foreach (ICompFloatMenuOptionsHandler handler in _childHandlers)
        {
            if (handler is ICompGetGizmosExtraHandler gizmoHandler)
            {
                gizmoHandler.AddGizmosExtra(builder, selectedPawn);
            }
        }
    }
}
