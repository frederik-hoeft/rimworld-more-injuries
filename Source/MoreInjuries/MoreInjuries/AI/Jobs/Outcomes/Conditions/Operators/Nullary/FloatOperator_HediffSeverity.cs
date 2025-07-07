using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Nullary;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_HediffSeverity : FloatOperator
{
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef? hediffDef = default;

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        if (hediffDef is null)
        {
            Logger.ConfigError($"Missing or invalid hediffDef");
            return 0f;
        }
        Hediff? hediff = patient.health.hediffSet.GetFirstHediffOfDef(hediffDef);
        if (hediff is null)
        {
            return 0f;
        }
        return hediff.Severity;
    }
}
