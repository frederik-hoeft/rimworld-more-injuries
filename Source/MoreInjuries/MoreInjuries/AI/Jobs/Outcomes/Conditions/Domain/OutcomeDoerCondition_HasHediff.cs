using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Domain;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class OutcomeDoerCondition_HasHediff : OutcomeDoerCondition
{
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef hediffDef = default!;

    public override bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) => 
        patient.health?.hediffSet.HasHediff(hediffDef) ?? false;
}