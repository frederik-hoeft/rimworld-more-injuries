using MoreInjuries.Defs;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlSerializable]
public sealed partial class JobOutcomeProperties_ModExtension_TemplateRef : JobOutcomeProperties_ModExtension
{
    [XmlMember("templateDef")]
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
