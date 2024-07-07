using Verse;

namespace MoreInjuries.Jobs;

public class chokingcompProperties : HediffCompProperties
{
    public int ABCD;
    public SoundDef coughSound;

    public chokingcompProperties()
    {
        compClass = typeof(chokingcomp);
    }

    public chokingcompProperties(Type compClass) : base()
    {
        this.compClass = compClass;
    }
}
