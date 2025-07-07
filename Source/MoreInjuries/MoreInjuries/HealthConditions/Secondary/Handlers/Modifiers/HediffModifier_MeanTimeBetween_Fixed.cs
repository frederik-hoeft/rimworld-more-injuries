using RimWorld;
using System.Threading;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class HediffModifier_MeanTimeBetween_Fixed : HediffModifier_MeanTimeBetween
{
    // don't rename this field. XML defs depend on this name
    private readonly float ticks = 0f;
    // don't rename this field. XML defs depend on this name
    private readonly float hours = 0f;
    // don't rename this field. XML defs depend on this name
    private readonly float days = 0f;
    // don't rename this field. XML defs depend on this name
    private readonly float quadrums = 0f;
    // don't rename this field. XML defs depend on this name
    private readonly float years = 0f;

    private float _mttf = -1f;

    private float Mttf
    {
        get
        {
            float mttf = Volatile.Read(ref _mttf);
            if (mttf < Mathf.Epsilon)
            {
                mttf = ticks
                    + (hours * GenDate.TicksPerHour)
                    + (days * GenDate.TicksPerDay)
                    + (quadrums * GenDate.TicksPerQuadrum)
                    + (years * GenDate.TicksPerYear);
                if (mttf <= Mathf.Epsilon)
                {
                    Logger.ConfigError($"{nameof(HediffModifier_MeanTimeBetween)} not properly initialized (ticks={ticks}, hours={hours}, days={days}, quadrums={quadrums}, years={years}). MTTF must be > 0. Defaulting to 1 day.");
                    mttf = GenDate.TicksPerDay; // default to 1 day if not set
                }
                Interlocked.Exchange(ref _mttf, mttf);
            }
            return mttf;
        }
    }

    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler) =>
        GetChanceFromMttf(Mttf, compHandler.TickInterval);
}
