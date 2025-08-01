using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Iterators.Enumerators;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_Enumerate_HediffSeverities : FloatOperator_Enumerate_Flat
{
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef? hediffDef = default;

    private HediffDef HediffDef
    {
        get
        {
            Throw.InvalidOperationException.IfNull(this, hediffDef);
            return hediffDef;
        }
    }

    protected override IEnumerable<float> FlatEnumerate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        HediffDef hediffDef = HediffDef;
        foreach (Hediff hediff in patient.health.hediffSet.hediffs)
        {
            if (hediff.def == hediffDef)
            {
                yield return hediff.Severity;
            }
        }
    }

    public override string ToString() => $"enumerate_hediff_severities({hediffDef?.defName ?? "null"})";
}