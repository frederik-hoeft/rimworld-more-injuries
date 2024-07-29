using RimWorld;
using Verse;

namespace MoreInjuries;

public class InjuriesCompProps : CompProperties
{
    // TODO: are these even used?
    public HediffDef Concussion;
    public HediffDef Shock;
    public NeedDef polak;

    public InjuriesCompProps()
    {
        compClass = typeof(InjuriesComp);
    }

    public InjuriesCompProps(Type compClass) : base(compClass)
    {
        this.compClass = compClass;
    }
}

