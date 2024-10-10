using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public class TourniquetHediffComp : HediffComp
{
    public List<BetterInjury>? Injuries { get; set; }

    public override void CompPostPostRemoved()
    {
        if (Injuries is not null)
        {
            foreach (BetterInjury injury in Injuries)
            {
                injury.IsBase = true;
                injury.IsCoagulationMultiplierApplied = false;
            }
            Injuries.Clear();
        }
        base.CompPostPostRemoved();
    }
}

