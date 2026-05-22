using MoreInjuries.Roslyn.Future.ThrowHelpers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Buffers;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[XmlBindable]
public partial class HediffMakerProperties_RandomFromList : HediffMakerProperties
{
    [XmlBinding("hediffMakerDefs", Validate = nameof(ValidateHediffMakerDefs))]
    public partial IReadOnlyList<HediffMakerDef> HediffMakerDefs { get; }

    private static bool ValidateHediffMakerDefs(IReadOnlyList<HediffMakerDef> defs) => defs is { Count: > 0 };

    public override HediffMakerDef GetHediffMakerDef(HediffComp parentComp, HediffCompHandler_SecondaryCondition handler, BodyPartRecord? targetBodyPart)
    {
        IReadOnlyList<HediffMakerDef> hediffMakerDefs = HediffMakerDefs;
        // for every evaluation, we need to build a cumulative distribution function (CDF) based on the weights of the hediff maker defs,
        // then generate a random value and find the corresponding index in the CDF
        // for that CDF, we can reuse the temporary buffer since the number of defs is unlikely to change frequently
        float[] cachedCdf = ArrayPool<float>.Shared.Rent(hediffMakerDefs.Count);
        // we have no idea how long the cached array is, so we clamp it to the working size to avoid out of bounds access
        Span<float> cdf = cachedCdf.AsSpan(0, hediffMakerDefs.Count);
        float totalWeight = 0f;
        // cdf.Length == hediffMakerDefs.Count, so we can safely iterate over either one
        for (int i = 0; i < cdf.Length; ++i)
        {
            float weight = 1f; // Default weight for non-weighted defs
            if (hediffMakerDefs[i] is WeightedHediffMakerDef weightedDef)
            {
                weight = weightedDef.Weight;
            }
            totalWeight += weight;
            cdf[i] = totalWeight;
        }
        float randomValue = Rand.Range(0f, totalWeight);
        int index = BinarySearch(cdf, randomValue);
        ArrayPool<float>.Shared.Return(cachedCdf);
        return hediffMakerDefs[index];
    }

    // Binary search to find the index of the first element greater than or equal to the target value
    // assumes that the array is sorted in ascending order and non-empty
    private static int BinarySearch(ReadOnlySpan<float> array, float target)
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
