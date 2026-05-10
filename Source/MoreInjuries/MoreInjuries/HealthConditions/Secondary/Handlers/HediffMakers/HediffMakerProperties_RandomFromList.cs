using MoreInjuries.Roslyn.Future.ThrowHelpers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[XmlBindable]
public partial class HediffMakerProperties_RandomFromList : HediffMakerProperties
{
    [ThreadStatic]
    private static float[]? t_cdfCache;

    [XmlBinding("hediffMakerDefs")]
    public partial IReadOnlyList<HediffMakerDef> HediffMakerDefs { get; }

    public override HediffMakerDef GetHediffMakerDef(HediffComp parentComp, HediffCompHandler_SecondaryCondition handler, BodyPartRecord? targetBodyPart)
    {
        IReadOnlyList<HediffMakerDef> hediffMakerDefs = HediffMakerDefs;
        t_cdfCache ??= new float[hediffMakerDefs.Count];
        Throw.InvalidOperationException.If(t_cdfCache.Length != hediffMakerDefs.Count);
        float totalWeight = 0f;
        for (int i = 0; i < hediffMakerDefs.Count; i++)
        {
            float weight = 1f; // Default weight for non-weighted defs
            if (hediffMakerDefs[i] is WeightedHediffMakerDef weightedDef)
            {
                weight = weightedDef.Weight;
            }
            totalWeight += weight;
            t_cdfCache[i] = totalWeight;
        }
        float randomValue = Rand.Range(0f, totalWeight);
        int index = BinarySearch(t_cdfCache, randomValue);
        if (index < 0 || index >= hediffMakerDefs.Count)
        {
            throw new InvalidOperationException($"{nameof(HediffMakerProperties_RandomFromList)}: Random index {index} is out of bounds for hediff maker defs list.");
        }
        return hediffMakerDefs[index];
    }

    // Binary search to find the index of the first element greater than or equal to the target value
    // assumes that the array is sorted in ascending order and non-empty
    private static int BinarySearch(float[] array, float target)
    {
        int low = 0;
        int high = array.Length - 1;
        while (low <= high)
        {
            int mid = (low + high) / 2;
            if (array[mid] < target)
            {
                low = mid + 1;
            }
            else if (array[mid] > target)
            {
                high = mid - 1;
            }
            else
            {
                return mid; // Found exact match
            }
        }
        return low; // Return the index of the first element greater than the target
    }
}
