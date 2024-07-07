using Verse;

namespace MoreInjuries;

public class ShockCompProps : HediffCompProperties
{
    public ShockCompProps() : base()
    {
        compClass = typeof(ShockComp);
    }

    public SimpleCurve BleedSeverityCurve;
}
