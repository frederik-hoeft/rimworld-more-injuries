using MoreInjuries.AI.Jobs.Outcomes.Conditions;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public abstract class JobOutcomeDoer
{
    // don't rename this field. XML defs depend on this name
    private readonly OutcomeDoerCondition? condition = null;

    public OutcomeDoerCondition? Condition => condition;

    public virtual bool TryDoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        if (Condition is null || Condition.ShouldDoOutcome(doctor, patient, device, runtimeState: null))
        {
            Logger.LogDebug($"Execute outcome for {doctor} treating {patient} with {device?.LabelCap ?? "no device"}: {this}");
            return DoOutcome(doctor, patient, device);
        }
        return true; // condition not met, outcome not applied => no error
    }

    protected abstract bool DoOutcome(Pawn doctor, Pawn patient, Thing? device);

    public abstract override string ToString();
}