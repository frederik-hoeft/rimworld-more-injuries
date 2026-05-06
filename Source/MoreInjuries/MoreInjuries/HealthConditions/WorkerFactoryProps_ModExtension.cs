using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions;

[XmlSerializable]
public sealed partial class WorkerFactoryProps_ModExtension : DefModExtension
{
    [XmlMember("workerFactories")]
    public partial IReadOnlyList<IInjuryWorkerFactory>? WorkerFactories { get; }
}
