using Verse;

namespace MoreInjuries;

public class ShockCompProps : HediffCompProperties
{
    public ShockCompProps() : base()
    {
        this.compClass = typeof(ShockComp);
    }

    public SimpleCurve BleedSeverityCurve;
}
