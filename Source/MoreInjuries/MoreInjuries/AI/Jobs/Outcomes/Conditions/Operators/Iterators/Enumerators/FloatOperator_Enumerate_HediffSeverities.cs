using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Iterators.Enumerators;

[XmlSerializable]
public sealed partial class FloatOperator_Enumerate_HediffSeverities : FloatOperator_Enumerate_Flat
{
    [XmlMember("hediffDef", AllowRawAccess = true)]
    private partial HediffDef HediffDef { get; }

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
