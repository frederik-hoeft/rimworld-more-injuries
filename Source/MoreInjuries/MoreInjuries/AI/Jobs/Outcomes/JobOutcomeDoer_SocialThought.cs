using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using RimWorld;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlBindable]
public sealed partial class JobOutcomeDoer_SocialThought : JobOutcomeDoer
{
    [XmlBinding("thoughtDef")]
    public partial ThoughtDef ThoughtDef { get; }

    [XmlBinding<bool>("ignoreHostilities", defaultValue: false)]
    public partial bool IgnoreHostilities { get; }

    protected override bool DoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        if (patient.needs.mood is { } mood && (IgnoreHostilities || !doctor.HostileTo(patient)))
        {
            mood.thoughts.memories.TryGainMemory(ThoughtDef, doctor);
        }
        return true;
    }

    public override string ToString() =>
        $"{nameof(JobOutcomeDoer_SocialThought)}(ThoughtDef: {ThoughtDef.defName})";
}
