using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using RimWorld;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlSerializable]
public sealed partial class JobOutcomeDoer_SocialThought : JobOutcomeDoer
{
    [XmlMember("thoughtDef")]
    public partial ThoughtDef ThoughtDef { get; }

    [XmlMember<bool>("ignoreHostilities", defaultValue: false)]
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
