using MoreInjuries.HealthConditions.Injectors.Epinephrine;
using Verse;

namespace MoreInjuries.HealthConditions.Injectors;

public class InjectorWorker : InjuryWorker, ICompFloatMenuOptionsHandler
{
    private readonly ICompFloatMenuOptionsHandler[] _childHandlers;

    public InjectorWorker(MoreInjuryComp parent) : base(parent)
    {
        _childHandlers =
        [
            new EpinephrineFloatOptionsProvider(this)
        ];
    }

    public override bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        foreach (ICompFloatMenuOptionsHandler handler in _childHandlers)
        {
            if (handler.IsEnabled)
            {
                handler.AddFloatMenuOptions(builder, selectedPawn);
            }
        }
    }
}
