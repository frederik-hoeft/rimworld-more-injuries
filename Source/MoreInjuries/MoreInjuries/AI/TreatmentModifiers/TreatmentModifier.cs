using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

[XmlBindable]
public abstract partial class TreatmentModifier
{
    [XmlBinding("jobDef")]
    public partial JobDef JobDef { get; }

    public abstract float GetEffectiveness(Hediff hediff);
}
