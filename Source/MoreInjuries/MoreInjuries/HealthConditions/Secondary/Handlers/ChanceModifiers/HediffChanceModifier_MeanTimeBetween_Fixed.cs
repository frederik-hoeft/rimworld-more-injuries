using MoreInjuries.BuildIntrinsics;
using RimWorld;
using System.Threading;
using UnityEngine;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class HediffChanceModifier_MeanTimeBetween_Fixed : HediffChanceModifier_MeanTimeBetween
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
                    Logger.ConfigError($"{nameof(HediffChanceModifier_MeanTimeBetween)} not properly initialized (ticks={ticks}, hours={hours}, days={days}, quadrums={quadrums}, years={years}). MTTF must be > 0. Defaulting to 1 day.");
                    mttf = GenDate.TicksPerDay; // default to 1 day if not set
                }
                Interlocked.Exchange(ref _mttf, mttf);
            }
            return mttf;
        }
    }

    public override float GetModifier(HediffComp_SecondaryCondition comp, HediffCompHandler_SecondaryCondition compHandler) =>
        GetChanceFromMttf(Mttf, compHandler.TickInterval);
}
