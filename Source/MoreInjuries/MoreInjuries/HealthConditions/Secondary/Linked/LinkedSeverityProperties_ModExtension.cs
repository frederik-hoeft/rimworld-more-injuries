using MoreInjuries.BuildIntrinsics;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Linked;

// members initialized via XML defs
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class LinkedSeverityProperties_ModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly List<HediffCompHandler_LinkedSeverity>? modifiedHediffs = default;
    private Dictionary<HediffDef, HediffCompHandler_LinkedSeverity>? _linkedSeverityHandlers__intrinsic = null;

    public IReadOnlyDictionary<HediffDef, HediffCompHandler_LinkedSeverity> LinkedSeverityHandlers
    {
        get
        {
            if (_linkedSeverityHandlers__intrinsic is not null)
            {
                return _linkedSeverityHandlers__intrinsic;
            }
            _linkedSeverityHandlers__intrinsic = [];
            if (modifiedHediffs is { Count: > 0 })
            {
                foreach (HediffCompHandler_LinkedSeverity handler in modifiedHediffs)
                {
                    _linkedSeverityHandlers__intrinsic[handler.LinkedHediffDef] = handler;
                }
            }
            return _linkedSeverityHandlers__intrinsic;
        }
    }
}
