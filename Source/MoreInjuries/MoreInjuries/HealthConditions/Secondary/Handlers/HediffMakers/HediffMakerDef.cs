using MoreInjuries.BuildIntrinsics;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffMakerDef(HediffDef hediffDef, float? minSeverity, float? maxSeverity, bool? allowDuplicate, bool? allowMultiple)
{
    // don't rename this field. XML defs depend on this name
    protected readonly HediffDef hediffDef = hediffDef;
    // don't rename this field. XML defs depend on this name
    protected readonly float? minSeverity = minSeverity;
    // don't rename this field. XML defs depend on this name
    protected readonly float? maxSeverity = maxSeverity;
    // don't rename this field. XML defs depend on this name
    protected readonly bool? allowDuplicate = allowDuplicate;
    // don't rename this field. XML defs depend on this name
    protected readonly bool? allowMultiple = allowMultiple;

    public HediffMakerDef() : this(default!, null, null, null, null) { }

    public HediffDef HediffDef => hediffDef ?? throw new InvalidOperationException($"{nameof(HediffMakerDef)}: {nameof(HediffDef)} is not set. Cannot evaluate.");

    public float MinSeverity => minSeverity ?? 0f;

    public float MaxSeverity => maxSeverity ?? 0f;

    public bool AllowDuplicate => allowDuplicate ?? false;

    public bool AllowMultiple => allowMultiple ?? false;

    public float MinSeverityOrDefault(float defaultValue = 0f) => minSeverity ?? defaultValue;

    public float MaxSeverityOrDefault(float defaultValue = 0f) => maxSeverity ?? defaultValue;

    public bool AllowDuplicateOrDefault(bool defaultValue = false) => allowDuplicate ?? defaultValue;

    public bool AllowMultipleOrDefault(bool defaultValue = false) => allowMultiple ?? defaultValue;

    public float GetInitialSeverity()
    {
        float minSeverity = MinSeverityOrDefault(defaultValue: 0f);
        float maxSeverity = MaxSeverityOrDefault(defaultValue: 0f);
        if (minSeverity == maxSeverity)
        {
            return minSeverity;
        }
        if (maxSeverity == 0f)
        {
            return minSeverity;
        }
        if (minSeverity > maxSeverity)
        {
            Logger.Error($"{nameof(HediffCompHandler_SecondaryCondition_TargetsBodyPart)}: {nameof(minSeverity)} ({minSeverity}) is greater than {nameof(maxSeverity)} ({maxSeverity}). Using {nameof(minSeverity)} instead.");
            return minSeverity;
        }
        return Rand.Range(minSeverity, maxSeverity);
    }
}
