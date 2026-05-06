using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlBindable]
public partial class JobOutcomeProperties_ModExtension : DefModExtension
{
    [XmlBinding("outcomeDoers")]
    public virtual partial IReadOnlyList<JobOutcomeDoer> OutcomeDoers { get; }
}
