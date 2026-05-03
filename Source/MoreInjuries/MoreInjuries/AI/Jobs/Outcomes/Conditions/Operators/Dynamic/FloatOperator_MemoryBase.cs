using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

[XmlSerializable]
public abstract partial class FloatOperator_MemoryBase(string? defaultSymbol) : FloatOperator
{
    [XmlMember("symbol", DefaultValueFrom = nameof(defaultSymbol))]
    protected partial string Symbol { get; }
}
