using MoreInjuries.Roslyn.Future.ThrowHelpers;
using RimWorld;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public abstract class JobOutcomeDoer_NeedBase : JobOutcomeDoer
{
    // don't rename this field. XML defs depend on this name
    private readonly NeedDef? needDef = default;

    public NeedDef NeedDef => Throw.InvalidOperationException.IfNull(this, needDef);

    protected abstract bool DoOutcome(Pawn doctor, Pawn patient, Thing? device, Need need);

    protected override bool DoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        NeedDef needDef = NeedDef;
        if (patient.needs.TryGetNeed(needDef, out Need? need))
        {
            return DoOutcome(doctor, patient, device, need);
        }
        return true;
    }

    public override string ToString() => $"{GetType().Name}: {NeedDef.defName}";
}