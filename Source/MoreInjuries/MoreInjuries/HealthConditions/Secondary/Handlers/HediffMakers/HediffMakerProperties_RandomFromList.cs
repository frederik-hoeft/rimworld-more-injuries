using MoreInjuries.Debug;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[XmlBindable]
public partial class HediffMakerProperties_RandomFromList : HediffMakerProperties
{
    [XmlBinding("hediffMakerDefs", Validate = nameof(ValidateHediffMakerDefs))]
    public partial IReadOnlyList<HediffMakerDef> HediffMakerDefs { get; }

    private CachedCdf Cdf
    {
        get
        {
            if (field is null)
            {
                float[] cdf = new float[HediffMakerDefs.Count];
                float totalWeight = 0f;
                for (int i = 0; i < cdf.Length; ++i)
                {
                    float weight = 1f; // Default weight for non-weighted defs
                    if (HediffMakerDefs[i] is WeightedHediffMakerDef weightedDef)
                    {
                        weight = weightedDef.Weight;
                        if (weight <= 0f)
                        {
                            Logger.Warning($"HediffMakerDef for {weightedDef.HediffDef.defName} has a zero-or-negative weight. Treating it as a small positive weight.");
                            weight = Mathf.Epsilon;
                        }
                    }
                    totalWeight += weight;
                    cdf[i] = totalWeight;
                }
                field = new CachedCdf(cdf, totalWeight);
            }
            return field;
        }
    }

    private static bool ValidateHediffMakerDefs(IReadOnlyList<HediffMakerDef> defs) => defs is { Count: > 0 };

    public override HediffMakerDef GetHediffMakerDef(HediffComp parentComp, HediffCompHandler_SecondaryCondition handler)
    {
        IReadOnlyList<HediffMakerDef> hediffMakerDefs = HediffMakerDefs;
        (float[] cdf, float totalWeight) = Cdf;
        Throw.InvalidOperationException.If(hediffMakerDefs.Count != cdf.Length, "Cached CDF length does not match the number of HediffMakerDefs. This should never happen.");
        float randomValue = Rand.Range(0f, totalWeight);
        int index = BinarySearch(cdf, randomValue);
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

    private sealed record CachedCdf(float[] Cdf, float TotalWeight);
}
