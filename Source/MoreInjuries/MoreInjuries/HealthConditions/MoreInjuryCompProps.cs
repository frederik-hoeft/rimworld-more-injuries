using Verse;

namespace MoreInjuries.HealthConditions;

public class MoreInjuryCompProps(Type compClass) : CompProperties(compClass)
{
    public MoreInjuryCompProps() : this(typeof(MoreInjuryComp)) { }
}