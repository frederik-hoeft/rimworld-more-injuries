using MoreInjuries.BuildIntrinsics;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class WorkerFactoryProps_ModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly List<IInjuryWorkerFactory>? workerFactories = default;

    public IReadOnlyList<IInjuryWorkerFactory>? WorkerFactories => workerFactories;
}