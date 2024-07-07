using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoreInjuries;

public class JobDriver_HardTending : JobDriver
{

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    public List<Thing> medicalDevices = new();

    protected override IEnumerable<Toil> MakeNewToils()
    {
        return DeviceToilsUtils.UseBleedDecreaser((Pawn)this.TargetA.Thing, GetActor());
    }
}
