using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

[XmlSerializable]
public sealed partial class TreatmentModifiers_ModExtension : DefModExtension
{
    [XmlMember("modifiers")]
    public partial IReadOnlyList<TreatmentModifier>? Modifiers { get; }

    private Dictionary<JobDef, TreatmentModifier[]> TreatmentModifiersByJobDef
    {
        get
        {
            if (Volatile.Read(ref field) is { } result)
            {
                return result;
            }
            Dictionary<JobDef, TreatmentModifier[]> treatmentModifiersByJobDef = Modifiers
                ?.GroupBy(modifier => modifier.JobDef)
                .ToDictionary(group => group.Key, group => group.ToArray())
                ?? [];
            if (Interlocked.CompareExchange(ref field, value: treatmentModifiersByJobDef, comparand: null) is { } concurrentResult)
            {
                // another thread already initialized the dictionary, so we return that one
                return concurrentResult;
            }
            // we initialized the dictionary, so we return it
            return treatmentModifiersByJobDef;
        }
    }

    public float GetTreatmentEffectiveness(JobDef jobDef, Hediff hediff)
    {
        float effectiveness = 1f;
        if (!TreatmentModifiersByJobDef.TryGetValue(jobDef, out TreatmentModifier[]? modifiers))
        {
            return effectiveness; // no modifiers, so default effectiveness
        }
        // we know that each modifiers array contains at least one element
        foreach (TreatmentModifier modifier in modifiers)
        {
            effectiveness *= modifier.GetEffectiveness(hediff);
        }
        return effectiveness;
    }
}
