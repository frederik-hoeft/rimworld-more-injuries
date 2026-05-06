using MoreInjuries.Defs;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions;

[XmlSerializable]
public partial class MoreInjuryCompProperties : CompProperties
{
    public MoreInjuryCompProperties() => compClass = typeof(MoreInjuryComp);

    [XmlMember("workerFactoryDef")]
    public partial ReferenceableDef WorkerFactoryDef { get; }

    public IReadOnlyList<IInjuryWorkerFactory> WorkerFactories
    {
        get
        {
            if (WorkerFactoryDef.GetModExtension<WorkerFactoryProps_ModExtension>() is { WorkerFactories: { Count: > 0 } factories })
            {
                return factories;
            }
            return Array.Empty<IInjuryWorkerFactory>();
        }
    }
}
