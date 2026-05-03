using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Domain;

[XmlSerializable]
public sealed partial class OutcomeDoerCondition_HasHediff : OutcomeDoerCondition
{
    [XmlMember("hediffDef")]
    public partial HediffDef HediffDef { get; }

    public override bool ShouldDoOutcome(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) => 
        patient.health?.hediffSet.HasHediff(HediffDef) ?? false;

    public override string ToString() => $"has_hediff({HediffDef.defName})";
}
