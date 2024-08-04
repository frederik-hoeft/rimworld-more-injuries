using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Hemostat;

public class HemostatHediffComp : HediffComp
{
    public BetterInjury? Injury { get; set; }

    public override void CompPostPostRemoved()
    {
        if (Injury is not null)
        {
            Injury.IsBase = true;
            Injury.IsHemostatApplied = false;
        }
        base.CompPostPostRemoved();
    }
}
