using MoreInjuries.Defs;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlBindable]
public sealed partial class JobOutcomeProperties_ModExtension_TemplateRef : JobOutcomeProperties_ModExtension
{
    [XmlBinding("templateDef")]
    public partial ReferenceableDef TemplateDef { get; }

    public override IReadOnlyList<JobOutcomeDoer> OutcomeDoers
    {
        get
        {
            if (field is null)
            {
                if (TemplateDef.GetModExtension<JobOutcomeProperties_ModExtension>() is not { } templateOutcomeDoers)
                {
                    throw new InvalidOperationException($"outcome doer template '{TemplateDef.defName}' has no {nameof(JobOutcomeProperties_ModExtension)}");
                }
                return field =
                [
                    .. templateOutcomeDoers.OutcomeDoers,
                    .. base.OutcomeDoers ?? []
                ];
            }
            return field;
        }
    }
}
