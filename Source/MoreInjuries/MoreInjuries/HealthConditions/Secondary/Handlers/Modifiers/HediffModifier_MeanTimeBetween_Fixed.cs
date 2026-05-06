using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using RimWorld;
using System.Threading;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlSerializable]
public sealed partial class HediffModifier_MeanTimeBetween_Fixed : HediffModifier_MeanTimeBetween
{
    [XmlMember<float>("ticks", defaultValue: 0f)]
    public partial float Ticks { get; }

    [XmlMember<float>("hours", defaultValue: 0f)]
    public partial float Hours { get; }

    [XmlMember<float>("days", defaultValue: 0f)]
    public partial float Days { get; }

    [XmlMember<float>("quadrums", defaultValue: 0f)]
    public partial float Quadrums { get; }

    [XmlMember<float>("years", defaultValue: 0f)]
    public partial float Years { get; }

    private float Mttf
    {
        get
        {
            float mttf = Volatile.Read(ref field);
            if (mttf < Mathf.Epsilon)
            {
                mttf = Ticks
                    + (Hours * GenDate.TicksPerHour)
                    + (Days * GenDate.TicksPerDay)
                    + (Quadrums * GenDate.TicksPerQuadrum)
                    + (Years * GenDate.TicksPerYear);
                if (mttf <= Mathf.Epsilon)
                {
                    Logger.ConfigError($"hediff modifier was not properly initialized (ticks={Ticks}, hours={Hours}, days={Days}, quadrums={Quadrums}, years={Years}). MTTF must be > 0. Defaulting to 1 day.");
                    mttf = GenDate.TicksPerDay; // default to 1 day if not set
                }
                Interlocked.Exchange(ref field, mttf);
            }
            return mttf;
        }
    } = -1f;

    public override float GetModifier(Hediff hediff, IHediffComp_TickHandler compHandler) =>
        GetChanceFromMttf(Mttf, compHandler.TickInterval);
}
