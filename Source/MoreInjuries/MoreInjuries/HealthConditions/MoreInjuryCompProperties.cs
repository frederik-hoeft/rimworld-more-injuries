using MoreInjuries.Defs;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
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