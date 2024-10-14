using MoreInjuries.KnownDefs;
using UnityEngine;
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
            KnownSoundDefOf.ChokingSound.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
        }

        base.CompPostMake();
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (_ticksThisInterval > 0)
        {
            _ticksThisInterval--;
        }
        if (_ticksThisInterval == 0)
        {
            _ticksThisInterval = Properties.ChokingIntervalTicks;
            if (Source is null)
            {
                Logger.Warning("Choking hediff has no source injury! Was this hediff added manually?");
            }
            else
            {
                // a random walk with a bias towards increasing severity, increase depends on the bleed rate of the source injury and whether the patient is tended
                float increase = 0.1f;
                float decrease = 0f;
                if (Source.BleedRate > 0.01f)
                {
                    increase += Mathf.Clamp(Source.BleedRate / 5f, 0.05f, 0.25f);
                }
                else if (Source.IsTended())
                {
                    decrease = 0.075f;
                }
                else if (Source.BleedRate <= 0.01f)
                {
                    decrease = 0.05f;
                }
                float change = Rand.Range(-decrease, increase);
                float newSeverity = Mathf.Clamp01(parent.Severity + change);
                if (newSeverity > 0f)
                {
                    parent.Severity = newSeverity;
                    if (MoreInjuriesMod.Settings.EnableChokingSounds)
                    {
                        KnownSoundDefOf.ChokingSound.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
                    }
                }
                else
                {
                    parent.pawn.health.RemoveHediff(parent);
                }
            }
        }

        base.CompPostTick(ref severityAdjustment);
    }
}
