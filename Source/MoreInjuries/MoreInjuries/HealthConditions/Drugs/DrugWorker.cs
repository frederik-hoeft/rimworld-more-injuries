using MoreInjuries.HealthConditions.Drugs.Chloroform;
using MoreInjuries.HealthConditions.Drugs.Epinephrine;
using MoreInjuries.HealthConditions.Drugs.Ketamine;
using MoreInjuries.HealthConditions.Drugs.Morphine;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs;

public class DrugWorker : InjuryWorker, ICompFloatMenuOptionsHandler
{
    private readonly ICompFloatMenuOptionsHandler[] _childHandlers;

    public DrugWorker(MoreInjuryComp parent) : base(parent)
    {
        _childHandlers =
        [
            new EpinephrineFloatOptionsProvider(this),
            new KetamineFloatOptionsProvider(this),
            new ChloroformFloatOptionsProvider(this),
            new MorphineFloatOptionsProvider(this),
        ];
    }

    public override bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        if (!selectedPawn.Drafted)
        {
            return;
        }
        foreach (ICompFloatMenuOptionsHandler handler in _childHandlers)
        {
            if (handler.IsEnabled)
            {
                handler.AddFloatMenuOptions(builder, selectedPawn);
            }
        }
    }
}
