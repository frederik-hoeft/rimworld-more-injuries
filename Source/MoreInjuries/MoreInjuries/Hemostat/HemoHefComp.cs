using Verse;

namespace MoreInjuries.Hemostat;

public class HemoHefComp : HediffComp
{
    public BetterInjury injur;

    public override void CompPostPostRemoved()
    {
        if (injur != null)
        {
            injur.isBase = true;

            injur.hemod = false;
        }
        base.CompPostPostRemoved();
    }
}
