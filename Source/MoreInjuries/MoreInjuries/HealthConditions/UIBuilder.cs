using System.Collections.Generic;

namespace MoreInjuries.HealthConditions;

public record UIBuilder<T>(HashSet<UITreatmentOption> Keys, List<T> Options)
{
    public void Clear()
    {
        Keys.Clear();
        Options.Clear();
    }
}