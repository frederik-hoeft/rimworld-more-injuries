using Verse;
using Verse.Sound;

namespace MoreInjuries.Jobs;

public class chokingcomp : HediffComp
{
    public MoreInjuriesSettings Settings
    {
        get
        {
            return LoadedModManager.GetMod<MoreInjuriesMod>().GetSettings<MoreInjuriesSettings>();
        }
    }
    public int chocke_int;

    public Hediff_Injury source;

    public override void CompPostMake()
    {
        chocke_int = Props.ABCD;
        if (Settings.somesound)
        {
            Props.coughSound.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
        }

        base.CompPostMake();
    }
    public override void CompPostTick(ref float severityAdjustment)
    {

        if (chocke_int != 1)
        {
            --chocke_int;
        }
        if (Rand.Chance(0.55f))
        {
            float change = 0.02f;
            if (source != null && source.BleedRate > 0.01f)
            {
                change = source.BleedRate / 5;
                ////
            }
            else
            {
                change = 0.10f;
            }
            if (chocke_int == 1)
            {
                if (Settings.somesound)
                {
                    Props.coughSound.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
                }
                parent.Severity += change;
                chocke_int = Props.ABCD;
            }
        }
        else
        {
            if (chocke_int == 1)
            {
                float change = 0.25f;
                if (Settings.somesound)
                {
                    Props.coughSound.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
                }
                if (source.IsTended())
                {
                    change = 0.11f;
                }
                parent.Severity -= change;
                chocke_int = Props.ABCD;
            }
        }

        base.CompPostTick(ref severityAdjustment);
    }

    public chokingcompProperties Props => (chokingcompProperties)props;

}
