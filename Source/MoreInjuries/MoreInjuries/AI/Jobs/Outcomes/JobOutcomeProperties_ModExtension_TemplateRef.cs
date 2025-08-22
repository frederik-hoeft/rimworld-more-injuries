using MoreInjuries.Defs;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Collections.Generic;

namespace MoreInjuries.AI.Jobs.Outcomes;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class JobOutcomeProperties_ModExtension_TemplateRef : JobOutcomeProperties_ModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly ReferenceableDef? templateDef = default;
    private JobOutcomeDoer[]? _outcomeDoers;

    public override IReadOnlyList<JobOutcomeDoer> OutcomeDoers
    {
        get
        {
            Throw.InvalidOperationException.IfNull(this, templateDef);
            if (_outcomeDoers is not null)
            {
                return _outcomeDoers;
            }
            if (templateDef.GetModExtension<JobOutcomeProperties_ModExtension>() is not { } templateOutcomeDoers)
            {
                throw new InvalidOperationException($"outcome doer template '{templateDef.defName}' has no {nameof(JobOutcomeProperties_ModExtension)}");
            }
            return _outcomeDoers =
            [
                .. templateOutcomeDoers.OutcomeDoers,
                .. outcomeDoers ?? []
            ];
        }
    }
}