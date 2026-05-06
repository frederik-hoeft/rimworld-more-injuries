using MoreInjuries.AI.Jobs.Outcomes.Conditions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlBindable]
public abstract partial class JobOutcomeDoer
{
    [XmlBinding("condition")]
    public partial OutcomeDoerCondition? Condition { get; }

    public virtual bool TryDoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        if (Condition is not { } condition || condition.ShouldDoOutcome(doctor, patient, device, runtimeState: null))
        {
            Logger.LogDebug($"Execute outcome for {doctor} treating {patient} with {device?.LabelCap ?? "no device"}: {this}");
            return DoOutcome(doctor, patient, device);
        }
        return true; // condition not met, outcome not applied => no error
    }

    protected abstract bool DoOutcome(Pawn doctor, Pawn patient, Thing? device);

    public abstract override string ToString();
}