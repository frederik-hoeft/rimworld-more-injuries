using Verse;
using Verse.Sound;

namespace MoreInjuries.HealthConditions.Choking;

public class ChokingHediffComp : HediffComp
{
    private int _ticksThisInterval;

    public Hediff_Injury? Source { get; set; }

    public ChokingHediffCompProperties Properties => (ChokingHediffCompProperties)props;

    public override void CompPostMake()
    {
        _ticksThisInterval = Properties.ChokingIntervalTicks;
        if (MoreInjuriesMod.Settings.EnableChokingSounds)
        {
            Properties.CoughSoundDef.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
        }

        base.CompPostMake();
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        // TODO: this logic seems weird. We randomly increase or decrease severity, and the decrease on tended is smaller than on untended.
        if (_ticksThisInterval > 0)
        {
            _ticksThisInterval--;
        }
        if (Rand.Chance(0.55f))
        {
            float change = 0.10f;
            if (Source?.BleedRate is > 0.01f)
            {
                change = Source.BleedRate / 5f;
            }
            if (_ticksThisInterval == 0)
            {
                if (MoreInjuriesMod.Settings.EnableChokingSounds)
                {
                    Properties.CoughSoundDef.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
                }
                parent.Severity += change;
                _ticksThisInterval = Properties.ChokingIntervalTicks;
            }
        }
        else if (_ticksThisInterval == 0)
        {
            if (MoreInjuriesMod.Settings.EnableChokingSounds)
            {
                Properties.CoughSoundDef.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
            }
            float change = 0.25f;
            if (Source.IsTended())
            {
                change = 0.11f;
            }
            parent.Severity -= change;
            _ticksThisInterval = Properties.ChokingIntervalTicks;
        }

        base.CompPostTick(ref severityAdjustment);
    }
}
