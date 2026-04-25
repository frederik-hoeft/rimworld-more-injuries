using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

[XmlSerializable]
public sealed partial class DynamicRuntimeProcedureDef_ModExtension : DefModExtension
{
    [XmlMember("instructions")]
    public partial List<FloatOperator>? Instructions { get; }
}
