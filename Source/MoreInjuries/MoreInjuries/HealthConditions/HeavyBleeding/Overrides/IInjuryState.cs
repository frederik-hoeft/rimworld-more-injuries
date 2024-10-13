namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides;

public interface IInjuryState
{
    CoagulationFlag CoagulationFlags { get; set; }

    float CoagulationMultiplier { get; set; }

    float EffectiveBleedRateMultiplier { get; }

    bool IsTemporarilyCoagulated { get; }

    int ReducedBleedRateTicksTotal { get; set; }

    float TemporarilyTamponadedMultiplierBase { get; set; }
}
