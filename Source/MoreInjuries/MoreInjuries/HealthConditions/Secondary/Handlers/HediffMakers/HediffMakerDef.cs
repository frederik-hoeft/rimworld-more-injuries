using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[XmlSerializable]
public partial class HediffMakerDef
{
    [XmlMember("hediffDef")]
    public partial HediffDef HediffDef { get; }

    [XmlMember("minSeverity")]
    public partial float MinSeverity { get; }

    [XmlMember("maxSeverity")]
    public partial float MaxSeverity { get; }

    [XmlMember("allowDuplicate")]
    public partial bool AllowDuplicate { get; }

    [XmlMember("allowMultiple")]
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
