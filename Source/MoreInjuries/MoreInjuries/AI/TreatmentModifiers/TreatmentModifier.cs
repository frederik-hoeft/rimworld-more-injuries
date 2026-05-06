using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

[XmlSerializable]
public abstract partial class TreatmentModifier
{
    [XmlMember("jobDef")]
    public partial JobDef JobDef { get; }

    public abstract float GetEffectiveness(Hediff hediff);
}
