namespace MoreInjuries.HealthConditions;

public readonly struct CoagulationFlag(int value)
{
    private readonly int _value = value;

    public static CoagulationFlag None => new(0);

    public static CoagulationFlag Timed => new(1);

    public static CoagulationFlag Manual => new(2);

    public bool IsEmpty => _value == 0;

    public static implicit operator int(CoagulationFlag context) => context._value;

    public static explicit operator CoagulationFlag(int value) => new(value);

    public static bool operator ==(CoagulationFlag left, CoagulationFlag right) => left._value == right._value;

    public static bool operator !=(CoagulationFlag left, CoagulationFlag right) => left._value != right._value;

    public bool IsSet(CoagulationFlag flag) => (_value & flag._value) == flag._value;

    public static CoagulationFlag Set(CoagulationFlag context, CoagulationFlag flag) => (CoagulationFlag)(context._value | flag._value);

    public static CoagulationFlag Unset(CoagulationFlag context, CoagulationFlag flag) => (CoagulationFlag)(context._value & ~flag._value);

    public static CoagulationFlag operator |(CoagulationFlag left, CoagulationFlag right) => new(left._value | right._value);

    public static CoagulationFlag operator &(CoagulationFlag left, CoagulationFlag right) => new(left._value & right._value);

    public static CoagulationFlag operator ~(CoagulationFlag flag) => new(~flag._value);

    public override bool Equals(object? obj) => obj is CoagulationFlag flag && _value == flag._value;

    public override int GetHashCode() => -1939223833 + _value.GetHashCode();
}