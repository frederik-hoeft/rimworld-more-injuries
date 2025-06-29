using MoreInjuries.BuildIntrinsics;
using MoreInjuries.HealthConditions.Drugs.Outcomes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class MedicalDrugProps_ModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly List<DrugOutcomeDoer> outcomeDoers = default!;

    public List<DrugOutcomeDoer> OutcomeDoers => outcomeDoers;
}