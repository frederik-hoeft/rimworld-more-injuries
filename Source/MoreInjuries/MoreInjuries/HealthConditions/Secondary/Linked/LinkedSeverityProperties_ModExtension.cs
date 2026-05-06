using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Linked;

[XmlSerializable]
public sealed partial class LinkedSeverityProperties_ModExtension : DefModExtension
{
    [XmlMember("modifiedHediffs")]
    public partial IReadOnlyList<HediffCompHandler_LinkedSeverity>? ModifiedHediffs { get; }

    public IReadOnlyDictionary<HediffDef, HediffCompHandler_LinkedSeverity> LinkedSeverityHandlers
    {
        get
        {
            if (field is not null)
            {
                return field;
            }
            Dictionary<HediffDef, HediffCompHandler_LinkedSeverity> linkedSeverityHandlers = [];
            if (ModifiedHediffs is { } modifiedHediffs)
            {
                foreach (HediffCompHandler_LinkedSeverity handler in modifiedHediffs)
                {
                    linkedSeverityHandlers[handler.LinkedHediffDef] = handler;
                }
            }
            return field = linkedSeverityHandlers;
        }
    } = null;
}
