using Verse;

namespace MoreInjuries.Hemostat;

public class HemostatHediffComp : HediffComp
{
    public BetterInjury Injury { get; set; }

    public override void CompPostPostRemoved()
    {
        if (Injury != null)
        {
            Injury.IsBase = true;
            Injury.IsHemostatApplied = false;
        }
        base.CompPostPostRemoved();
    }
}
