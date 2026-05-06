using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.BrainDamage;

[XmlBindable]
public sealed partial class BrainDamageTreatmentProps_ModExtension : DefModExtension
{
    [XmlBinding("severityReductionRange", DefaultValueProvider = typeof(FloatRange), DefaultValueFrom = nameof(FloatRange.Zero))]
    public partial FloatRange SeverityReductionRange { get; }

    [XmlBinding("daysToComplete", Transform = nameof(CapDaysToComplete))]
    public partial float DaysToComplete { get; }

    private static float CapDaysToComplete(float value) => Mathf.Max(0.1f, value);
}
