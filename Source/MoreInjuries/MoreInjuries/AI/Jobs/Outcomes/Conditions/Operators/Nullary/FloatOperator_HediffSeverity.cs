using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Nullary;

[XmlBindable]
public sealed partial class FloatOperator_HediffSeverity() : FloatOperator
{
    [XmlBinding("hediffDef")]
    public partial HediffDef HediffDef { get; init; }

    internal FloatOperator_HediffSeverity(HediffDef hediffDef) : this() => HediffDef = hediffDef;

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        HediffDef hediffDef = HediffDef;
        Hediff? hediff = patient.health.hediffSet.GetFirstHediffOfDef(hediffDef);
        if (hediff is null)
        {
            Logger.LogDebug($"No hediff of def {hediffDef.defName} found on {patient}. Returning 0 severity.");
            return 0f;
        }
        return hediff.Severity;
    }

    public override string ToString() => $"hediff_severity({HediffDef.defName})";
}
