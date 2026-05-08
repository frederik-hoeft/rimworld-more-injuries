using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions;

[XmlBindable]
public sealed partial class WorkerFactoryProps_ModExtension : DefModExtension
{
    [XmlBinding("workerFactories")]
    public partial IReadOnlyList<IInjuryWorkerFactory>? WorkerFactories { get; }
}
