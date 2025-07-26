using UnityEngine;

namespace MoreInjuries.HealthConditions.Hemodilution;

public static class HemodilutionEvaluator
{
    public static float CalculateMaximumSafeAddedSalineVolume(float hemodulutionSeverity, float bloodLossSeverity, float hemodilutionThreshold)
    {
        // map blood loss = 1 (death) to 50% of total volume lost
        float currentVolume = 1f - (bloodLossSeverity * 0.5f);
        return currentVolume * (hemodulutionSeverity - hemodilutionThreshold) / (hemodilutionThreshold - 1);
    }

    public static int CalculateMaximumSafeSalineTransfusions(float hemodulutionSeverity, float bloodLossSeverity, float hemodilutionThreshold, float salineBagVolume)
    {
        float maxAllowedAddedVolume = CalculateMaximumSafeAddedSalineVolume(hemodulutionSeverity, bloodLossSeverity, hemodilutionThreshold);
        int maxAllowedTransfusions = Mathf.FloorToInt(maxAllowedAddedVolume / salineBagVolume);
        return maxAllowedTransfusions;
    }

    public static float CalculateMaxNeededWholeBloodVolumeToTreatHemodilution(float hemodulutionSeverity, float bloodLossSeverity)
    {
        // map blood loss = 1 (death) to 50% of total volume lost
        float missingVolume = bloodLossSeverity * 0.5f;
        float currentVolume = 1f - missingVolume;
        float minRequiredAddedVolume = missingVolume + (hemodulutionSeverity * currentVolume / (currentVolume + missingVolume));
        return minRequiredAddedVolume;
    }

    public static int CalculateMaxNeededBloodTransfusionsToTreatHemodilution(float hemodulutionSeverity, float bloodLossSeverity, float bloodBagVolume)
    {
        float minRequiredAddedVolume = CalculateMaxNeededWholeBloodVolumeToTreatHemodilution(hemodulutionSeverity, bloodLossSeverity);
        return Mathf.CeilToInt(minRequiredAddedVolume / bloodBagVolume);
    }

    public static float CalculateMinimumRequiredWholeBloodVolumeToTreatHemodilution(float hemodulutionSeverity, float bloodLossSeverity, float hemodilutionThreshold)
    {
        if (hemodulutionSeverity < hemodilutionThreshold)
        {
            return 0f;
        }
        // map blood loss = 1 (death) to 50% of total volume lost
        float missingVolume = bloodLossSeverity * 0.5f;
        float currentVolume = 1f - missingVolume;
        // valid for 0 <= x <= missingVolume
        float x = (hemodulutionSeverity * currentVolume / hemodilutionThreshold) - currentVolume;
        if (0 <= x && x <= missingVolume)
        {
            return x;
        }
        // x > missingVolume (overflow required)
        x = missingVolume + (hemodulutionSeverity * currentVolume / (currentVolume + missingVolume)) - hemodilutionThreshold;
        return x;
    }

    public static int CalculateMinimumRequiredBloodTransfusionsToTreatHemodilution(float hemodulutionSeverity, float bloodLossSeverity, float hemodilutionThreshold, float bloodBagVolume)
    {
        float minRequiredAddedVolume = CalculateMinimumRequiredWholeBloodVolumeToTreatHemodilution(hemodulutionSeverity, bloodLossSeverity, hemodilutionThreshold);
        return Mathf.CeilToInt(minRequiredAddedVolume / bloodBagVolume);
    }
}
