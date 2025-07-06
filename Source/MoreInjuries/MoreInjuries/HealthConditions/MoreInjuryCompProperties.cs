using MoreInjuries.BuildIntrinsics;
using MoreInjuries.Defs;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class MoreInjuryCompProperties : CompProperties
{
    private readonly ReferenceableDef workerFactoryDef = default!;

    public MoreInjuryCompProperties() => compClass = typeof(MoreInjuryComp);

    public ReferenceableDef WorkerFactoryDef => workerFactoryDef;

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