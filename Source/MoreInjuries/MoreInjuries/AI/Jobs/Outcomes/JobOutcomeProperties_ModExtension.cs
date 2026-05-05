using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlSerializable]
public partial class JobOutcomeProperties_ModExtension : DefModExtension
{
    [XmlMember("outcomeDoers")]
    public virtual partial IReadOnlyList<JobOutcomeDoer> OutcomeDoers { get; }
}
