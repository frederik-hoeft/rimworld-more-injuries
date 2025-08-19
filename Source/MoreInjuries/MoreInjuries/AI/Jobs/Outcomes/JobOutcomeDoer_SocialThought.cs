using MoreInjuries.Roslyn.Future.ThrowHelpers;
using RimWorld;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class JobOutcomeDoer_SocialThought : JobOutcomeDoer
{
    // don't rename this field. XML defs depend on this name
    private readonly ThoughtDef thoughtDef = default!;
    // don't rename this field. XML defs depend on this name
    private readonly bool ignoreHostilities = false;

    protected override bool DoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        Throw.InvalidOperationException.IfNull(this, thoughtDef);
        if (patient.needs.mood is { } mood && (ignoreHostilities || !doctor.HostileTo(patient)))
        {
            mood.thoughts.memories.TryGainMemory(thoughtDef, doctor);
        }
        return true;
    }

    public override string ToString() =>
        $"{nameof(JobOutcomeDoer_SocialThought)}(ThoughtDef: {thoughtDef.defName})";
}
