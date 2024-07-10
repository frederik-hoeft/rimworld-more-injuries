using System.Collections.Generic;
using Verse;

namespace MoreInjuries;

public static class DeviceToilsUtils
{
    public static List<Thing> ThingsInRange(this Thing vecsource)
    {
        List<Thing> result = [];
        foreach (IntVec3 vec3 in vecsource.CellsAdjacent8WayAndInside())
        {
            if (vec3.GetThingList(vecsource.Map) is List<Thing> { Count: > 0 } things)
            {
                result.AddRange(things);
            }
        }
        return result;
    }
}
