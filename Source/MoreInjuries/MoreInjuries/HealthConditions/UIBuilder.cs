using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions;

public record UIBuilder<T>(HashSet<UITreatmentOption> Keys, List<T> Options)
{
    public void Clear()
    {
        Keys.Clear();
        Options.Clear();
    }

    public void AddOptionIfResearched(ResearchProjectDef researchProject, Func<T> optionFactory)
    {
        if (researchProject.IsFinished)
        {
            Options.Add(optionFactory());
        }
    }
}