using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using RimWorld;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlBindable]
public abstract partial class JobOutcomeDoer_NeedBase : JobOutcomeDoer
{
    [XmlBinding("needDef")]
    public partial NeedDef NeedDef { get; }

    protected abstract bool DoOutcome(Pawn doctor, Pawn patient, Thing? device, Need need);

    protected override bool DoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        if (patient.needs.TryGetNeed(NeedDef, out Need? need))
        {
            return DoOutcome(doctor, patient, device, need);
        }
        return true;
    }

    public override string ToString() => $"{GetType().Name}: {NeedDef.defName}";
}
