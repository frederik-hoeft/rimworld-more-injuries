using MoreInjuries.BuildIntrinsics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class TreatmentModifiers_ModExtension : DefModExtension
{
    // do not rename this field. XML defs depend on this name
    private readonly List<TreatmentModifier>? modifiers = default;

    private Dictionary<JobDef, TreatmentModifier[]>? _treatmentModifiersByJobDef;

    private Dictionary<JobDef, TreatmentModifier[]> TreatmentModifiersByJobDef
    {
        get
        {
            if (Volatile.Read(ref _treatmentModifiersByJobDef) is { } result)
            {
                return result;
            }
            Dictionary<JobDef, TreatmentModifier[]> treatmentModifiersByJobDef = modifiers is not null
                ? modifiers
                    .GroupBy(modifier => modifier.JobDef)
                    .ToDictionary(group => group.Key, group => group.ToArray())
                : [];
            if (Interlocked.CompareExchange(ref _treatmentModifiersByJobDef, value: treatmentModifiersByJobDef, comparand: null) is { } concurrentResult)
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