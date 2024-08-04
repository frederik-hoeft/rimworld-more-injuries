using Verse;

namespace MoreInjuries.HealthConditions;

public class InjuryCompProps(Type compClass) : CompProperties(compClass)
{
    public InjuryCompProps() : this(typeof(InjuryComp)) { }
}

