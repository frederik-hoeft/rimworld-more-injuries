using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffMakerProperties_RandomFromList : HediffMakerProperties
{
    [ThreadStatic]
    private static float[]? t_cdfCache;

    // don't rename this field. XML defs depend on this name
    private readonly List<HediffMakerDef> hediffMakerDefs = default!;
    // don't rename this field. XML defs depend on this name
    private readonly float minSeverityDefault = 0f;
    // don't rename this field. XML defs depend on this name
    private readonly float maxSeverityDefault = 0f;
    // don't rename this field. XML defs depend on this name
    private readonly bool allowDuplicateDefault = false;
    // don't rename this field. XML defs depend on this name
    private readonly bool allowMultipleDefault = false;

    public override HediffMakerDef GetHediffMakerDef(HediffComp parentComp, HediffCompHandler_SecondaryCondition handler, BodyPartRecord? targetBodyPart)
    {
        if (hediffMakerDefs is not { Count: > 0 })
        {
            throw new InvalidOperationException($"{nameof(HediffMakerProperties_RandomFromList)}: {parentComp.GetType().Name} has no hediff maker defs defined. Cannot evaluate.");
        }
        t_cdfCache ??= new float[hediffMakerDefs.Count];
        if (t_cdfCache.Length != hediffMakerDefs.Count)
        {
            throw new InvalidOperationException($"{nameof(HediffMakerProperties_RandomFromList)}: CDF cache length {t_cdfCache.Length} does not match hediff maker defs count {hediffMakerDefs.Count}.");
        }
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
        HediffMakerDef selectedDef = hediffMakerDefs[index];
        // apply defaults if not set
        return new HediffMakerDef
        (
            selectedDef.HediffDef,
            selectedDef.MinSeverityOrDefault(minSeverityDefault),
            selectedDef.MaxSeverityOrDefault(maxSeverityDefault),
            selectedDef.AllowDuplicateOrDefault(allowDuplicateDefault),
            selectedDef.AllowMultipleOrDefault(allowMultipleDefault)
        );
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