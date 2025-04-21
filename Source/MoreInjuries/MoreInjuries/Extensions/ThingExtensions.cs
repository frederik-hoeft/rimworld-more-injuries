using System.Collections.Generic;
using Verse;

namespace MoreInjuries.Extensions;

public static class ThingExtensions
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

    /// <summary>
    /// Decreases the stack count of the <paramref name="thing"/> by one. If the resulting stack count is less than or equal to zero, the <paramref name="thing"/> is destroyed.
    /// </summary>
    /// <param name="thing">The <see cref="Thing"/> to decrease the stack count of.</param>
    /// <returns><see langword="true"/> if the <paramref name="thing"/> was destroyed, <see langword="false"/> otherwise.</returns>
    public static bool DecreaseStack(this Thing thing)
    {
        if (thing.stackCount <= 1)
        {
            thing.Destroy();
            return true;
        }
        thing.stackCount--;
        return false;
    }
}
