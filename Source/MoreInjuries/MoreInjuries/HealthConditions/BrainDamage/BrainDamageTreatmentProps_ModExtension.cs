using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.BrainDamage;

[XmlSerializable]
public sealed partial class BrainDamageTreatmentProps_ModExtension : DefModExtension
{
    [XmlMember("severityReductionRange", DefaultValueProvider = typeof(FloatRange), DefaultValueFrom = nameof(FloatRange.Zero))]
    public partial FloatRange SeverityReductionRange { get; }

    [XmlMember("daysToComplete", Transform = nameof(CapDaysToComplete))]
    public partial float DaysToComplete { get; }

    private static float CapDaysToComplete(float value) => Mathf.Max(0.1f, value);
}
