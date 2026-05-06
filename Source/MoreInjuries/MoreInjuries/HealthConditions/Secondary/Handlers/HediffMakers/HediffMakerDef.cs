using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[XmlBindable]
public partial class HediffMakerDef
{
    [XmlBinding("hediffDef")]
    public partial HediffDef HediffDef { get; }

    [XmlBinding("minSeverity")]
    public partial float MinSeverity { get; }

    [XmlBinding("maxSeverity")]
    public partial float MaxSeverity { get; }

    [XmlBinding("allowDuplicate")]
    public partial bool AllowDuplicate { get; }

    [XmlBinding("allowMultiple")]
    public partial bool AllowMultiple { get; }

    public float GetInitialSeverity()
    {
        float minSeverity = MinSeverity;
        float maxSeverity = MaxSeverity;
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
            Logger.Error($"{nameof(minSeverity)} ({minSeverity}) is greater than {nameof(maxSeverity)} ({maxSeverity}). Using {nameof(minSeverity)} instead.");
            return minSeverity;
        }
        return Rand.Range(minSeverity, maxSeverity);
    }
}
